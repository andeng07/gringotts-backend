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
/// Handles client registration.
/// </summary>
/// <remarks>
/// This endpoint allows new clients to register by providing their personal details 
/// and credentials. If registration is successful, the client's information is stored, 
/// and a response is returned. If the username is already taken, a conflict response is returned.
/// </remarks>
/// <response code="201">Returns the newly registered client's details.</response>
/// <response code="409">If an account with the given username already exists.</response>
/// <response code="500">If an unexpected error occurs during registration.</response>
public class ClientRegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup("/auth").MapPost("/register", HandleAsync)
            .WithRequestValidation<RegisterUserRequest>()
            .WithAuthenticationFilter()
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
        
        var fileExtension = Path.GetExtension(request.UserProfileImage.FileName);
        var filePath = Path.Combine("wwwroot/images", user.Id + fileExtension); 

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.UserProfileImage.CopyToAsync(stream, cancellationToken); 
        }

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
        string Password,
        IFormFile UserProfileImage
    );

    private record RegisterUserResponse(
        Guid Id,
        string Username,
        string FirstName,
        string? MiddleName,
        string LastName
    );
}