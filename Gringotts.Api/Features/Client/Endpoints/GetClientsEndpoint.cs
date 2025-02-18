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
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/clients/filter", async ([FromBody] GetClientsRequest request,
                    AppDbContext dbContext) =>
                {
                    var dataFilter = new ClientFilter(request.SearchTerm).ApplyFilters();

                    return await EndpointHelpers.GetEntities<Models.Client, GetClientEndpoint.GetClientResponse>(
                        dbContext,
                        request.Page,
                        request.PageSize,
                        entityQuery: dataFilter,
                        responseMapper: c =>
                            new GetClientEndpoint.GetClientResponse(c.Id, c.FirstName, c.MiddleName, c.LastName)
                    );
                }
            )
            .WithAuthenticationFilter()
            .Produces<PaginatedResult<GetClientEndpoint.GetClientResponse>>();
    }

    public record GetClientsRequest(int Page, int PageSize, string? SearchTerm);
}