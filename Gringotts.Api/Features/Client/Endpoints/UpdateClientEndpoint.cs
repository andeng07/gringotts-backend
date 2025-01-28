using FluentValidation;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Client.Endpoints;

public class UpdateClientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("clients/{id:guid}",
                async ([FromBody] UpdateClientRequest request, Guid id, AppDbContext dbContext) =>
                await EndpointHelpers.UpdateEntity<Models.Client, UpdateClientResponse>(id, dbContext, updateEntity:
                    client =>
                    {
                        client.FirstName = request.FirstName;
                        client.MiddleName = request.MiddleName;
                        client.LastName = request.LastName;
                    },
                    responseMapper: client =>
                        new UpdateClientResponse(client.Id, client.FirstName, client.MiddleName, client.LastName)
                )
            )
            .WithAuthenticationFilter()
            .WithRequestValidation<UpdateClientRequest>();
    }

    public class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequest>
    {
        public UpdateClientRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .WithErrorCode(ValidationErrorCodes.FirstNameRequired);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .WithErrorCode(ValidationErrorCodes.LastNameRequired);
        }
    }

    public record UpdateClientRequest(
        string FirstName,
        string? MiddleName,
        string LastName
    );

    private record UpdateClientResponse(
        Guid Id,
        string FirstName,
        string? MiddleName,
        string LastName
    );
}