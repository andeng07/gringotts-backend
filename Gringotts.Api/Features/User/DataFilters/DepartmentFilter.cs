using System.Linq.Expressions;
using Gringotts.Api.Features.User.Models;
using Gringotts.Api.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.User.DataFilters;

public class DepartmentFilter(string? searchTerm) : IDataFilter<Department>
{
    public Expression<Func<Department, bool>> ApplyFilters()
    {
        var filter = new DataFilter<Department>();

        // If there's a search term, apply the search filters for building and room
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Build search query with multiple terms using DataFilter
            foreach (var term in terms)
            {
                filter.Where(c =>
                    EF.Functions.ILike(
                        (c.Name).Trim(),
                        $"%{term}%"));
            }
        }

        // Return the final predicate
        return filter.Build();
    }
}