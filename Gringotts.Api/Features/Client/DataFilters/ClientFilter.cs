using System.Linq.Expressions;
using Gringotts.Api.Shared.Utilities;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.Client.DataFilters;

public class ClientFilter(string? searchTerm) : IDataFilter<Models.Client>
{
    public Expression<Func<Models.Client, bool>> ApplyFilters()
    {
        var predicate = PredicateBuilder.New<Models.Client>(c => true); // Default: no filter

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Build search query with multiple terms using PredicateBuilder
            predicate = terms
                .Select(term => PredicateBuilder.New<Models.Client>(c =>
                    EF.Functions.ILike(
                        (c.FirstName + " " + (c.MiddleName ?? "") + " " + c.LastName).Trim(),
                        $"%{term}%"))).Aggregate(predicate, (current, searchExpression) => current.And(searchExpression)
                );
        }

        return predicate;
    }
}