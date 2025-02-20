using System.Linq.Expressions;
using Gringotts.Api.Features.Populator;
using Gringotts.Api.Shared.Utilities;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.Reader.DataFilters;

public class ReaderFilter(string? searchTerm, IEnumerable<Guid>? guids) : IDataFilter<Models.Reader>
{
    public Expression<Func<Models.Reader, bool>> ApplyFilters()
    {
        var filter = new DataFilter<Models.Reader>();

        // Apply the search filter if a search term is provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Build search query with multiple terms using DataFilter
            foreach (var term in terms)
            {
                filter.Where(c =>
                    EF.Functions.ILike(
                        c.Name.Trim(),
                        $"%{term}%"));
            }
        }

        // If guids are provided, apply the filter for guids (if needed)
        if (guids?.Any() == true)
        {
            filter.Where(c => guids.Contains(c.Id));  // Assuming the `Reader` model has an `Id` property
        }

        // Return the final predicate
        return filter.Build();
    }
}