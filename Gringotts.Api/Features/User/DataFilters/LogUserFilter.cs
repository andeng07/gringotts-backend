using System.Linq.Expressions;
using Gringotts.Api.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.User.DataFilters;

public class LogUserFilter(bool? expired, string? name, string? cardId, string? schoolId, IEnumerable<byte>? affiliations, IEnumerable<byte>? sexes, IEnumerable<Guid>? departments) : IDataFilter<Models.User>
{
    public Expression<Func<Models.User, bool>> ApplyFilters()
    {
        var filter = new DataFilter<Models.User>();

        if (expired.HasValue)
        {
            filter.Where(c => DateTime.UtcNow >= c.AccessExpiry == expired.Value);
        }

        // Apply name filter if provided
        if (!string.IsNullOrWhiteSpace(name))
        {
            var terms = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in terms)
            {
                filter.Where(c =>
                    EF.Functions.ILike(
                        (c.FirstName + " " + (c.MiddleName ?? "") + " " + c.LastName).Trim(),
                        $"%{term}%"));
            }
        }

        // Apply cardId filter if provided
        if (!string.IsNullOrWhiteSpace(cardId))
        {
            filter.Where(c => c.CardId == cardId);
        }

        // Apply schoolId filter if provided
        if (!string.IsNullOrWhiteSpace(schoolId))
        {
            filter.Where(c => c.SchoolId == schoolId);
        }

        // Apply affiliations filter if provided
        if (affiliations?.Any() == true)
        {
            filter.Where(c => affiliations.Contains(c.Affiliation));
        }

        // Apply sexes filter if provided
        if (sexes?.Any() == true)
        {
            filter.Where(c => sexes.Contains(c.Sex));
        }

        // Apply departments filter if provided
        if (departments?.Any() == true)
        {
            filter.Where(c => departments.Contains(c.DepartmentId ?? Guid.Empty));  // Assuming 'DepartmentId' is a nullable Guid
        }


        // Return the final combined predicate
        return filter.Build();
    }
}