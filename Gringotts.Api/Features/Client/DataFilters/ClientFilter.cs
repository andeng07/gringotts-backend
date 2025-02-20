using System.Linq.Expressions;
using Gringotts.Api.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.Client.DataFilters;

public class ClientFilter(string? searchTerm) : IDataFilter<Models.Client>
{
    public Expression<Func<Models.Client, bool>> ApplyFilters()
    {
        var filter = new DataFilter<Models.Client>();

        // If there's a search term, apply the search filters for name
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Build search query with multiple terms using PredicateBuilder
            foreach (var term in terms)
            {
                filter.Where(c =>
                    EF.Functions.ILike(
                        (c.FirstName + " " + (c.MiddleName ?? "") + " " + c.LastName).Trim(),
                        $"%{term}%"));
            }
        }

        // Return the final predicate
        return filter.Build();
    }
}