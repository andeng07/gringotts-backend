using System.Linq.Expressions;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Shared.Utilities;
using NPOI.SS.Formula.Functions;

namespace Gringotts.Api.Features.Interactions.DataFilters;

public class SessionsFilter(
    IEnumerable<Guid>? userIds,
    IEnumerable<Guid>? readerIds,
    DateTime? from,
    DateTime? to
) : IDataFilter<Session>
{
    public Expression<Func<Session, bool>> ApplyFilters()
    {
        var filter = new DataFilter<Session>();

        if (userIds?.Any() == true)
        {
            filter.Where(session => userIds.Contains(session.LogUserId));
        }

        if (readerIds?.Any() == true)
        {
            filter.Where(session => readerIds.Contains(session.LogReaderId));
        }
        
        if (from.HasValue)
        {
            filter.Where(log => log.StartDate >= from.Value);
        }

        if (to.HasValue)
        {
            filter.Where(log => log.EndDate <= to.Value);
        }

        return filter.Build();
    }
}