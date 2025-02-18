using Gringotts.Api.Features.Interactions.DataFilters;
using Gringotts.Api.Features.Interactions.Endpoints;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Shared.Core;
using Microsoft.EntityFrameworkCore;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;

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

        return GenerateExcelFile(data);
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
    
    private static IResult GenerateExcelFile(IEnumerable<ExportTableData> data)
{
    const string templatePath = "wwwroot/report-templates/SessionsTemplate.xlsx";

    if (!File.Exists(templatePath))
        return Results.Problem("Template not found.", statusCode: 404);

    using var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
    var workbook = new XSSFWorkbook(templateStream);

    // Use SXSSFWorkbook to handle large data sets with a buffer of 1000 rows
    var sxssfWorkbook = new SXSSFWorkbook(workbook, 1000); 
    var sheet = sxssfWorkbook.GetSheetAt(0);

    var startRow = 12; // Starting row after the header

    // Loop through the data and create rows
    foreach (var record in data)
    {
        // Try to get the row at the current index
        var row = sheet.GetRow(startRow);

        // If the row doesn't exist, create it
        if (row == null)
        {
            row = sheet.CreateRow(startRow);
        }

        // Edit the cells in the row if they exist, otherwise create them
        row.GetCell(0).SetCellValue(record.Reader);
        row.GetCell(1).SetCellValue(record.User);
        row.GetCell(2).SetCellValue(record.UserCard);
        row.GetCell(3).SetCellValue(record.Date);

        // Move to the next row
        startRow++;
    }

    // Write the workbook to a memory stream
    using var memoryStream = new MemoryStream();
    sxssfWorkbook.Write(memoryStream); // Write the workbook to the memory stream
    memoryStream.Flush();
    memoryStream.Position = 0; // Rewind the memory stream before returning

    sxssfWorkbook.Dispose(); // Dispose of the workbook to free memory

    // Return the memory stream as a downloadable file
    return Results.File(memoryStream.ToArray(), 
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
        "SessionsReport.xlsx");
}


    private record ExportTableData(string Reader, string User, string UserCard, string Date);
}