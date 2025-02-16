using Gringotts.Api.Features.Client.DataFilters;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Pagination;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

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
                    var dataFilter = new ClientFilter(searchTerm).ApplyFilters();

                    return await EndpointHelpers.GetEntities<Models.Client, GetClientEndpoint.GetClientResponse>(
                        dbContext,
                        page,
                        pageSize,
                        entityQuery: dataFilter,
                        responseMapper: c =>
                            new GetClientEndpoint.GetClientResponse(c.Id, c.CreatedAt, c.FirstName, c.MiddleName, c.LastName)
                    );
                }
            )

            //.WithAuthenticationFilter()
            .Produces<PaginatedResult<GetClientEndpoint.GetClientResponse>>();
    }
}