using System.Linq.Expressions;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Pagination;
using Gringotts.Api.Shared.Utilities;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.Client.Endpoints;

/// <summary>
/// Retrieves a paginated list of clients.
/// </summary>
/// <remarks>
/// This endpoint allows authenticated users to fetch a paginated list of clients. 
/// The results are retrieved based on the provided page number and page size.
/// </remarks>
/// <param name="page">The page number of the results to retrieve.</param>
/// <param name="pageSize">The number of clients per page.</param>
/// <response code="200">Returns a paginated list of clients.</response>
public class GetClientsEndpoint : IEndpoint
{
    // TODO COMPLEX SEARCH EACH FIELD OR ADD A NEW ENDPOINT (GET /clients/filter WITH BODY SO THAT IT GETS MUCH EASIER)
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/clients", async ([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? searchTerm,
                    AppDbContext dbContext) =>
                {
                    var predicate = PredicateBuilder.New<Models.Client>(c => true); // Default: no filter

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        // Build search query with multiple terms using PredicateBuilder
                        foreach (var term in terms)
                        {
                            var searchExpression = PredicateBuilder.New<Models.Client>(c =>
                                EF.Functions.ILike((c.FirstName + " " + (c.MiddleName ?? "") + " " + c.LastName).Trim(), $"%{term}%")
                            );

                            predicate = predicate.And(searchExpression); // Combine with AND
                        }
                    }

                    return await EndpointHelpers.GetEntities<Models.Client, GetClientEndpoint.GetClientResponse>(
                        dbContext,
                        page,
                        pageSize,
                        entityQuery: predicate,
                        responseMapper: c =>
                            new GetClientEndpoint.GetClientResponse(c.Id, c.CreatedAt, c.FirstName, c.MiddleName, c.LastName)
                    );
                }
            )

            //.WithAuthenticationFilter()
            .Produces<PaginatedResult<GetClientEndpoint.GetClientResponse>>();
    }
}