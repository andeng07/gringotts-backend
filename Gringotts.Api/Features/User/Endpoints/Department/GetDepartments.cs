using Gringotts.Api.Features.Reader.Endpoints.Readers;
using Gringotts.Api.Features.User.DataFilters;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Pagination;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.User.Endpoints.Department;

public class GetDepartments: IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/departments/filter", async ([FromBody] GetDepartmentsRequest request,
                    AppDbContext dbContext) =>
                {
                    var dataFilter = new DepartmentFilter(request.SearchTerm).ApplyFilters();

                    return await EndpointHelpers.GetEntities<Models.Department, GetDepartment.GetDepartmentResponse>(
                        dbContext,
                        request.Page,
                        request.PageSize,
                        entityQuery: dataFilter,
                        responseMapper: c =>
                            new GetDepartment.GetDepartmentResponse(c.Id, c.Name)
                    );
                }
            )
            // .WithAuthenticationFilter()
            .Produces<PaginatedResult<GetReaderEndpoint.GetReaderResponse>>();
    }

    public record GetDepartmentsRequest(
        int Page,
        int PageSize,
        string? SearchTerm
    );
}