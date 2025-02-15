using FluentValidation;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Client.Endpoints;

/// <summary>
/// Updates client details by their unique identifier.
/// </summary>
/// <remarks>
/// This endpoint allows authenticated users to update a client's information 
/// using their GUID. The request must include a valid first and last name. 
/// If validation fails, an error response is returned.
/// </remarks>
/// <param name="id">The unique identifier of the client to update.</param>
/// <param name="request">The updated client details.</param>
/// <response code="200">Returns the updated client details.</response>
/// <response code="400">If the request is invalid or fails validation.</response>
/// <response code="404">If no client with the specified ID exists.</response>
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