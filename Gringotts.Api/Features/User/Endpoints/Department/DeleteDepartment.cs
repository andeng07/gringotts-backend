using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;

namespace Gringotts.Api.Features.User.Endpoints.Department;

public class DeleteDepartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("departments/{id:guid}",
            async (Guid id, AppDbContext dbContext) =>
                await EndpointHelpers.DeleteEntity<Models.Department, DeleteDepartmentResponse>(id, dbContext,
                    responseMapper: department => new DeleteDepartmentResponse(department.Id, department.Name))
        )
        .WithAuthenticationFilter();
    }

    public record DeleteDepartmentResponse(Guid Id, string Name);
}