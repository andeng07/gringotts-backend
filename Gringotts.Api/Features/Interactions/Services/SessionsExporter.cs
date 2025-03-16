using Gringotts.Api.Features.Interactions.DataFilters;
using Gringotts.Api.Features.Interactions.Endpoints;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Shared.Core;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Gringotts.Api.Features.Interactions.Services;

public class SessionsExporter(AppDbContext dbContext)
{
    public async Task<IResult> ExportAsync(ExportSessionsEndpoint.ExportSessionsRequest request)
    {
        var sessions = await GetFilteredSessionsAsync(request);
        var users = await GetUsersAsync(sessions);
        var readers = await GetReadersAsync(sessions);

        var data = sessions.Select(session => new ExportTableData(
            readers.GetValueOrDefault(session.LogReaderId, "Not Found"),
            FormatUserName(users.GetValueOrDefault(session.LogUserId)),
            users.GetValueOrDefault(session.LogUserId)?.CardId ?? "Not Found",
            FormatDateRange(session.StartDate, session.EndDate)
        )).ToList();

        return await GenerateExcelFileAsync(data);
    }

    private async Task<List<Session>> GetFilteredSessionsAsync(ExportSessionsEndpoint.ExportSessionsRequest request)
    {
        var filter = new SessionsFilter(request.UserIds, request.ReaderIds, request.From, request.To).ApplyFilters();
        return await dbContext.Sessions
            .AsNoTracking()
            .Where(filter)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    private async Task<Dictionary<Guid, User.Models.User>> GetUsersAsync(IEnumerable<Session> sessions)
    {
        var userIds = sessions.Select(s => s.LogUserId).Distinct();
        return await dbContext.LogUsers
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id);
    }

    private async Task<Dictionary<Guid, string>> GetReadersAsync(IEnumerable<Session> sessions)
    {
        var readerIds = sessions.Select(s => s.LogReaderId).Distinct();
        return await dbContext.Readers
            .Where(r => readerIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.Name);
    }

    private static string FormatUserName(User.Models.User? user) =>
        user is null ? "Not Found" : $"{user.FirstName} {user.MiddleName} {user.LastName}";

    private static string FormatDateRange(DateTime? from, DateTime? to) =>
        from.HasValue && to.HasValue ? $"{from:yyyy-MM-dd HH:mm:ss} to {to:yyyy-MM-dd HH:mm:ss}" : "All Time";
    
    public static async Task<IResult> GenerateExcelFileAsync(IEnumerable<ExportTableData> data)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Required for EPPlus in .NET Core

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sessions");

        // Create the header row
        worksheet.Cells[1, 1].Value = "Reader";
        worksheet.Cells[1, 2].Value = "User";
        worksheet.Cells[1, 3].Value = "User Card";
        worksheet.Cells[1, 4].Value = "Date";

        int rowIndex = 2;
        foreach (var record in data)
        {
            worksheet.Cells[rowIndex, 1].Value = record.Reader;
            worksheet.Cells[rowIndex, 2].Value = record.User;
            worksheet.Cells[rowIndex, 3].Value = record.UserCard;
            worksheet.Cells[rowIndex, 4].Value = record.Date;
            rowIndex++;
        }

        var memoryStream = new MemoryStream();
        await package.SaveAsAsync(memoryStream);
    
        // Reset stream position before returning the file
        memoryStream.Position = 0;

        return Results.File(memoryStream, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            "SessionsReport.xlsx");
    }

    public record ExportTableData(string Reader, string User, string UserCard, string Date);
}
