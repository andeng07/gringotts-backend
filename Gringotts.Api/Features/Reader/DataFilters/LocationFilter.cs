using System.Linq.Expressions;
using Gringotts.Api.Features.Reader.Models;
using Gringotts.Api.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.Reader.DataFilters;

public class LocationFilter(string? searchTerm) : IDataFilter<Location>
{
    public Expression<Func<Location, bool>> ApplyFilters()
    {
        var filter = new DataFilter<Location>();

        // If there's a search term, apply the search filters for building and room
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Build search query with multiple terms using DataFilter
            foreach (var term in terms)
            {
                filter.Where(c =>
                    EF.Functions.ILike(
                        (c.BuildingName + " " + (c.RoomName ?? "")).Trim(),
                        $"%{term}%"));
            }
        }

        // Return the final predicate
        return filter.Build();
    }
}