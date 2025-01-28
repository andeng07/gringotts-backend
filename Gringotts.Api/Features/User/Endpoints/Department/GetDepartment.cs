using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.User.Endpoints.Department;

public class GetDepartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("departments/{id:guid}",
            async (Guid id, AppDbContext dbContext) =>
                await EndpointHelpers.GetEntity<Models.Department, GetDepartmentResponse>(id, dbContext,
                    responseMapper: department => new GetDepartmentResponse(department.Id, department.Name)
                )
        )
        .WithAuthenticationFilter()
        .Produces<GetDepartmentResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }

    public record GetDepartmentResponse(Guid Id, string Name);
}