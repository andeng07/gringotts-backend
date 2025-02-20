using FluentValidation;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.User.Endpoints.Department;

public class AddDepartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("departments/",
                async ([FromBody] AddDepartmentRequest request, AppDbContext dbContext) =>
                await EndpointHelpers.CreateEntity<Models.Department, AddDepartmentRequest>(
                    new Models.Department
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        Name = request.Name
                    },
                    dbContext,
                    department => $"/departments/{department.Id}"
                )
            )
            .WithAuthenticationFilter()
            .WithRequestValidation<AddDepartmentRequest>()
            .Produces<AddDepartmentRequest>();
    }

    public class AddDepartmentRequestValidator : AbstractValidator<AddDepartmentRequest>
    {
        public AddDepartmentRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
    
    public record AddDepartmentRequest(string Name);

    public record AddDepartmentResponse(Guid Id, string Name);
}