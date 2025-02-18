using Gringotts.Api.Features.Interactions.DataFilters;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SessionStats : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/stats/interactions",
            async (AppDbContext dbContext, [FromBody] GetInteractionLogsMonthlyStatsRequest request) =>
            {
                var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

                // Filter for all logs in the current month at once in UTC
                var dataFilter = new InteractionLogsFilter(
                    request.UserIds,
                    request.ReaderIds,
                    startOfMonth,
                    endOfMonth,
                    null
                );

                // Perform the grouping and counting in a single operation with optimized query
                var dailyCounts = await dbContext.InteractionLogs
                    .AsNoTracking() // No need for tracking, since it's read-only
                    .Where(dataFilter.ApplyFilters())
                    .GroupBy(log => new { log.DateTime.Date, log.InteractionType }) // Group by Date and InteractionType
                    .Select(group => new 
                    {
                        Date = group.Key.Date, // Use Date without formatting
                        InteractionType = group.Key.InteractionType,
                        Count = group.Count() // Count per InteractionType
                    })
                    .OrderBy(group => group.Date) // Order by Date
                    .ToListAsync(); // Execute the query and retrieve the results

                // Now format the Date and aggregate the counts by Date for each interaction type
                var dailyStats = dailyCounts
                    .GroupBy(x => x.Date.ToString("dd MMM")) // Format Date here in memory
                    .Select(group => new InteractionLogCountByDate(
                        group.Key, 
                        group.Where(x => x.InteractionType == InteractionType.Entry).Sum(x => x.Count),
                        group.Where(x => x.InteractionType == InteractionType.Exit).Sum(x => x.Count),
                        group.Where(x => x.InteractionType == InteractionType.Unauthorized).Sum(x => x.Count),
                        group.Where(x => x.InteractionType == InteractionType.Fallback).Sum(x => x.Count)
                    ))
                    .ToList();

                return new GetInteractionLogsMonthlyStatsResponse(dailyStats);
            }
        )
        .WithAuthenticationFilter();
    }

    // New class for better serialization
    public record InteractionLogCountByDate(
        string Date,
        int EntryCount,
        int ExitCount,
        int UnauthorizedCount,
        int FallbackCount
    );

    public record GetInteractionLogsMonthlyStatsRequest(
        IEnumerable<Guid>? UserIds,
        IEnumerable<Guid>? ReaderIds
    );

    public record GetInteractionLogsMonthlyStatsResponse(
        IEnumerable<InteractionLogCountByDate> DailyInteractionLogsCount
    );
}
