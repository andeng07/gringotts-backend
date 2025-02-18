using Gringotts.Api.Features.Reader.DataFilters;
using Gringotts.Api.Features.Reader.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Pagination;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Reader.Endpoints.Locations;

public class GetLocationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/locations/filter", async ([FromBody] GetLocationsRequest request,
                    AppDbContext dbContext) =>
                {
                    var dataFilter = new LocationFilter(request.SearchTerm).ApplyFilters();

                    return await EndpointHelpers.GetEntities<Location, GetLocationEndpoint.GetLocationResponse>(
                        dbContext,
                        request.Page,
                        request.PageSize,
                        entityQuery: dataFilter,
                        responseMapper: c =>
                            new GetLocationEndpoint.GetLocationResponse(c.Id, c.BuildingName, c.RoomName)
                    );
                }
            )
            .WithAuthenticationFilter()
            .Produces<PaginatedResult<GetLocationEndpoint.GetLocationResponse>>();
    }
    
    public record GetLocationsRequest(
        int Page,
        int PageSize,
        string? SearchTerm
    );
}