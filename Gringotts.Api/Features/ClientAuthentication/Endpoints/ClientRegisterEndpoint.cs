using FluentValidation;
using Gringotts.Api.Features.Client.Services;
using Gringotts.Api.Features.ClientAuthentication.Services;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.ClientAuthentication.Endpoints;

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
public class ClientRegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup("/auth").MapPost("/register", HandleAsync)
            .WithRequestValidation<RegisterUserRequest>()
            .Produces<RegisterUserResponse>(StatusCodes.Status201Created)
            .Produces<List<ErrorResponse>>(StatusCodes.Status409Conflict)
            .Produces<List<ErrorResponse>>(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] RegisterUserRequest request,
        ClientSecretService clientSecretService,
        ClientService clientService,
        CancellationToken cancellationToken
    )
    {
        // Check if the email is already registered
        if (await clientSecretService.HasSecretAsync(request.Username))
        {
            var errors = new List<ErrorResponse>
            {
                new(
                    AuthErrorCodes.EmailAlreadyRegistered, // todo
                    "An account with this email address already exists.",
                    ErrorResponse.ErrorType.Conflict
                )
            };
            return Results.Json(errors, statusCode: StatusCodes.Status409Conflict);
        }

        // Create user
        var registeredUserResult = await clientService.CreateUserAsync(
            request.FirstName,
            request.LastName,
            request.MiddleName
        );

        if (!registeredUserResult.IsSuccess)
        {
            return Results.Json(registeredUserResult.Errors, statusCode: StatusCodes.Status500InternalServerError);
        }

        // Create secret (password)
        var createSecretResult = await clientSecretService.CreateSecretAsync(
            registeredUserResult.Value!.Id,
            request.Username,
            request.Password
        );

        if (!createSecretResult.IsSuccess)
        {
            return Results.BadRequest(createSecretResult.Errors);
        }

        var secret = createSecretResult.Value!;
        var user = registeredUserResult.Value!;

        var response =
            new RegisterUserResponse(user.Id, secret.Username, user.FirstName, user.MiddleName, user.LastName);

        return Results.Created($"/users/{registeredUserResult.Value.Id}", response);
    }

    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.")
                .WithErrorCode(ValidationErrorCodes.EmailRequired);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .WithErrorCode(ValidationErrorCodes.FirstNameRequired);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .WithErrorCode(ValidationErrorCodes.LastNameRequired);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .WithErrorCode(ValidationErrorCodes.PasswordRequired)
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.")
                .WithErrorCode(ValidationErrorCodes.PasswordTooShort);
        }
    }

    public record RegisterUserRequest(
        string Username,
        string FirstName,
        string? MiddleName,
        string LastName,
        string Password
    );

    private record RegisterUserResponse(
        Guid Id,
        string Username,
        string FirstName,
        string? MiddleName,
        string LastName
    );
}