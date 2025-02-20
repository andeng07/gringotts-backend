using System.Linq.Expressions;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.Interactions.DataFilters;

public class ActiveSessionFilter(IEnumerable<Guid>? userIds, IEnumerable<Guid>? readerIds, DateTime? from, DateTime? to)
    : IDataFilter<ActiveSession>
{
    public Expression<Func<ActiveSession, bool>> ApplyFilters()
    {
        var filter = new DataFilter<ActiveSession>();

        if (userIds?.Any() == true)
        {
            filter.Where(log => userIds.Contains(log.LogUserId));
        }

        if (readerIds?.Any() == true)
        {
            filter.Where(log => readerIds.Contains(log.LogReaderId));
        }

        if (from.HasValue)
        {
            filter.Where(log => log.StartDate >= from.Value);
        }

        if (to.HasValue)
        {
            filter.Where(log => log.StartDate <= to.Value);
        }

        return filter.Build();
    }
}
