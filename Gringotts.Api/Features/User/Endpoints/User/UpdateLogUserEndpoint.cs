using FluentValidation;
using Gringotts.Api.Features.User.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.User.Endpoints.User;

/// <summary>
/// Endpoint for deleting a user.
/// </summary>
/// <remarks>
/// This endpoint allows the deletion of a user by their unique identifier.
/// The request must be authenticated, and the user must have the appropriate permissions.
/// </remarks>
/// <response code="200">Returns the deleted user's information if the operation is successful.</response>
/// <response code="404">Returns a not found error if the user does not exist.</response>
public class UpdateLogUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/{id:guid}", async ([FromBody] UpdateLogUserRequest request, Guid id, AppDbContext dbContext) =>
                await
                    EndpointHelpers.UpdateEntity<Models.User, UpdateLogUserResponse>(id, dbContext,
                        updateEntity: logUser =>
                        {
                            logUser.CardId = request.CardId;
                            logUser.SchoolId = request.SchoolId;
                            logUser.FirstName = request.FirstName;
                            logUser.MiddleName = request.MiddleName;
                            logUser.Affiliation = request.Affiliation;
                            logUser.LastName = request.LastName;
                        },
                        responseMapper: logUser => new UpdateLogUserResponse(
                            logUser.Id,
                            logUser.CardId,
                            logUser.SchoolId,
                            logUser.FirstName,
                            logUser.MiddleName,
                            logUser.LastName,
                            logUser.Affiliation,
                            logUser.Sex
                        )
                    )
            )
            .WithAuthenticationFilter()
            .WithRequestValidation<UpdateLogUserRequest>()
            .Produces<UpdateLogUserResponse>()
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    public class UpdateLogUserRequestValidator : AbstractValidator<UpdateLogUserRequest>
    {
        public UpdateLogUserRequestValidator()
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
            
            RuleFor(x => x.Sex)
                .GreaterThanOrEqualTo((byte) 0)
                .WithMessage("Sex must be greater than or equal to 0."); // todo error code
        }
    }

    public record UpdateLogUserRequest(
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName,
        byte Affiliation,
        byte Sex
    );

    public record UpdateLogUserResponse(
        Guid Id,
        string CardId,
        string SchoolId,
        string FirstName,
        string? MiddleName,
        string LastName,
        byte Affiliation,
        byte Sex
    );
}