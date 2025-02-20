using Gringotts.Api.Features.Reader.DataFilters;
using Gringotts.Api.Features.Reader.Endpoints.Locations;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Pagination;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Reader.Endpoints.Readers;

public class GetReadersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/readers/filter", async ([FromBody] GetReadersRequest request, AppDbContext dbContext) =>
                {
                    var dataFilter = new ReaderFilter(request.SearchTerm, request.Locations).ApplyFilters();

                    return await EndpointHelpers.GetEntities<Models.Reader, GetReaderEndpoint.GetReaderResponse>(
                        dbContext,
                        request.Page,
                        request.PageSize,
                        entityQuery: dataFilter,
                        responseMapper: c =>
                            new GetReaderEndpoint.GetReaderResponse(c.Id, c.Name, c.LocationId)
                    );
                }
            )
            .WithAuthenticationFilter()
            .Produces<PaginatedResult<GetReaderEndpoint.GetReaderResponse>>();
    }

    public record GetReadersRequest(
        int Page,
        int PageSize,
        string? SearchTerm,
        IEnumerable<Guid>? Locations
    );
}