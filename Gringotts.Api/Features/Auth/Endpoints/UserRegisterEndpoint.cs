using FluentValidation;
using Gringotts.Api.Features.Auth.Services;
using Gringotts.Api.Features.User.Services;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Extensions;
using Gringotts.Api.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Auth.Endpoints;

/// <summary>
/// Endpoint for handling user registration.
/// </summary>
/// <remarks>
/// This endpoint registers a new user by creating a user record and associated credentials.
/// It returns the user's ID and email upon successful registration, or a 409 Conflict error if the email is already registered.
/// </remarks>
/// <response code="201">Returns the user's ID and email upon successful registration.</response>
/// <response code="409">Returns a conflict error if the email is already registered.</response>
/// <response code="500">Returns a generic error response if an unexpected server error occurs.</response>
public class UserRegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup("/auth").MapPost("/register", HandleAsync)
            .WithRequestValidation<RegisterUserRequest>()
            .Produces<RegisterUserResponse>()
            .Produces<List<Error>>(StatusCodes.Status409Conflict)
            .Produces<List<Error>>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RegisterUserRequest request,
        UserSecretService userSecretService,
        UserService userService,
        CancellationToken cancellationToken
    )
    {
        // Check if the email is already registered
        if (await userSecretService.HasSecretAsync(request.Email))
        {
            var errors = new List<Error>
            {
                new(
                    AuthErrorCodes.EmailAlreadyRegistered,
                    "An account with this email address already exists.",
                    Error.ErrorType.Conflict
                )
            };
            return Results.Json(errors, statusCode: StatusCodes.Status409Conflict);
        }

        // Create user
        var registeredUserResult = await userService.CreateUserAsync(
            request.CardId,
            request.SchoolId,
            request.FirstName,
            request.LastName,
            request.MiddleName
        );

        if (!registeredUserResult.IsSuccess)
        {
            return Results.Json(registeredUserResult.Errors, statusCode: StatusCodes.Status500InternalServerError);
        }

        // Create secret (password)
        var createSecretResult = await userSecretService.CreateSecretAsync(
            registeredUserResult.Value!.Id,
            request.Email,
            request.Password
        );

        if (!createSecretResult.IsSuccess)
        {
            return Results.BadRequest(createSecretResult.Errors);
        }

        var response = new RegisterUserResponse(registeredUserResult.Value.Id, createSecretResult.Value!.Email);

        return Results.Created($"/users/{registeredUserResult.Value.Id}", response);
    }

    private class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.").WithErrorCode(ValidationErrorCodes.EmailRequired)
                .EmailAddress().WithMessage("Invalid email format.").WithErrorCode(ValidationErrorCodes.InvalidEmailFormat);

            RuleFor(x => x.CardId)
                .NotEmpty().WithMessage("Card ID is required.").WithErrorCode(ValidationErrorCodes.CardIdRequired);

            RuleFor(x => x.SchoolId)
                .NotEmpty().WithMessage("School ID is required.").WithErrorCode(ValidationErrorCodes.SchoolIdRequired);

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.").WithErrorCode(ValidationErrorCodes.FirstNameRequired);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.").WithErrorCode(ValidationErrorCodes.LastNameRequired);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.").WithErrorCode(ValidationErrorCodes.PasswordRequired)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.").WithErrorCode(ValidationErrorCodes.PasswordTooShort);
        }
    }

    private record RegisterUserRequest(
        string Email,
        string CardId,
        string SchoolId,
        string FirstName,
        string LastName,
        string Password,
        string? MiddleName = null
    );

    private record RegisterUserResponse(Guid UserId, string Email);
}
