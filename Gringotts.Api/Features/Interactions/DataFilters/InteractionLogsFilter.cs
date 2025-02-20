using System.Linq.Expressions;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.Interactions.DataFilters;

public class InteractionLogsFilter(
    IEnumerable<Guid>? userIds,
    IEnumerable<Guid>? readerIds,
    DateTime? from,
    DateTime? to,
    IEnumerable<InteractionType>? interactionTypes
) : IDataFilter<InteractionLog>
{
    public Expression<Func<InteractionLog, bool>> ApplyFilters()
    {
        var filter = new DataFilter<InteractionLog>();

        // Filter by UserIds (if provided)
        if (userIds?.Any() == true)
        {
            filter.Where(log => userIds.Contains(log.LogUserId ?? Guid.Empty));
        }

        // Filter by ReaderIds (if provided)
        if (readerIds?.Any() == true)
        {
            filter.Where(log => readerIds.Contains(log.LogReaderId));
        }

        // Filter by Date Range (if provided)
        if (from.HasValue)
        {
            filter.Where(log => log.DateTime >= from.Value);
        }

        if (to.HasValue)
        {
            filter.Where(log => log.DateTime <= to.Value);
        }

        // Filter by InteractionType (if provided)
        if (interactionTypes?.Any() == true)
        {
            filter.Where(log => interactionTypes.Contains(log.InteractionType));
        }

        return filter.Build();
    }
}