using FluentValidation;
using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Endpoints.Helper;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Extensions;
using Gringotts.Api.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.User.Endpoints;

/// <summary>
/// Endpoint for deleting a user.
/// </summary>
/// <remarks>
/// This endpoint allows the deletion of a user by their unique identifier.
/// The request must be authenticated, and the user must have the appropriate permissions.
/// </remarks>
/// <response code="200">Returns the deleted user's information if the operation is successful.</response>
/// <response code="404">Returns a not found error if the user does not exist.</response>
public class UpdateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/{id:guid}", async ([FromBody] UpdateUserRequest request, Guid id, AppDbContext dbContext) =>
                await
                    EndpointHelpers.UpdateEntity<Models.User, UpdateUserResponse>(id, dbContext,
                        updateEntity: entity =>
                        {
                            entity.CardId = request.CardId;
                            entity.SchoolId = request.SchoolId;
                            entity.FirstName = request.FirstName;
                            entity.MiddleName = request.MiddleName;
                            entity.LastName = request.LastName;
                        },
                        responseMapper: entity => new UpdateUserResponse(
                            entity.Id,
                            entity.CardId,
                            entity.SchoolId,
                            entity.FirstName,
                            entity.LastName,
                            entity.MiddleName
                        )
                    )
            )
            .WithRequestValidation<UpdateUserRequest>()
            .WithAuthenticationFilter()
            .Produces<UpdateUserResponse>()
            .Produces<Error>(StatusCodes.Status404NotFound);
    }

    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.CardId)
                .NotEmpty()
                    .WithMessage("Card ID is required.")
                    .WithErrorCode(ValidationErrorCodes.CardIdRequired);

            RuleFor(x => x.SchoolId)
                .NotEmpty()
                    .WithMessage("School ID is required.")
                    .WithErrorCode(ValidationErrorCodes.SchoolIdRequired);

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

    public record UpdateUserRequest(
        string CardId,
        string SchoolId,
        string FirstName,
        string LastName,
        string? MiddleName = null
    );

    public record UpdateUserResponse(
        Guid Id,
        string CardId,
        string SchoolId,
        string FirstName,
        string LastName,
        string? MiddleName = null
    );
}