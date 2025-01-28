using FluentValidation;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.User.Endpoints.Department;

public class UpdateDepartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("departments/{id:guid}",
            async ([FromBody] UpdateDepartmentRequest request, Guid id, AppDbContext dbContext) =>
            await EndpointHelpers.UpdateEntity<Models.Department, UpdateDepartmentResponse>(
                id,
                dbContext,
                updateEntity: department => department.Name = request.Name,
                responseMapper: department => new UpdateDepartmentResponse(department.Id, department.Name)
            )
        )
        .WithAuthenticationFilter()
        .WithRequestValidation<UpdateDepartmentRequest>()
        .Produces<UpdateDepartmentResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }

    public class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
    {
        public UpdateDepartmentRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public record UpdateDepartmentRequest(string Name);

    public record UpdateDepartmentResponse(Guid Id, string Name);
}