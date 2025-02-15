using FluentValidation;
using Gringotts.Api.Features.ClientAuthentication.Services;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.ClientAuthentication.Endpoints;

/// <summary>
/// Handles client authentication and login.
/// </summary>
/// <remarks>
/// This endpoint allows clients to authenticate using their credentials. Upon successful 
/// authentication, a JWT token is generated and returned. If authentication fails, 
/// an unauthorized response with error details is returned.
/// </remarks>
/// <response code="200">Returns a JWT token and the client ID upon successful login.</response>
/// <response code="401">If authentication fails due to invalid credentials.</response>
public class ClientLoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup("/auth").MapPost("/login", HandleAsync)
            .WithRequestValidation<LoginUserRequest>()
            .Produces<LoginUserResponse>()
            .Produces<List<ErrorResponse>>(StatusCodes.Status401Unauthorized);
    }
    
    public static async Task<IResult> HandleAsync(
        [FromBody] LoginUserRequest request,
        ClientSecretService clientSecretService,
        JwtService jwtService
    )
    {
        var passwordCheckResult = await clientSecretService.MatchSecretAsync(request.Username, request.Password);

        if (!passwordCheckResult.IsSuccess)
        {
            var errors = new List<ErrorResponse>
            {
                new(
                    AuthErrorCodes.CredentialsInvalid,
                    "Invalid email or password.",
                    ErrorResponse.ErrorType.Validation
                )
            };
            return Results.Json(errors, statusCode: StatusCodes.Status401Unauthorized);
        }

        var loggedInUserId = passwordCheckResult.Value;
        var token = jwtService.GenerateToken(loggedInUserId);

        return Results.Ok(new LoginUserResponse(token, loggedInUserId));
    }
    
    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                    .WithMessage("Username is required.")
                    .WithErrorCode(ValidationErrorCodes.EmailRequired);
            
            RuleFor(x => x.Password)
                .NotNull()
                    .WithMessage("Password is required.")
                    .WithErrorCode(ValidationErrorCodes.PasswordRequired)
                .NotEmpty()
                    .WithMessage("Password is required.")
                    .WithErrorCode(ValidationErrorCodes.PasswordRequired)
                .MinimumLength(6)
                    .WithMessage("Password must be at least 6 characters long.")
                    .WithErrorCode(ValidationErrorCodes.PasswordTooShort);
        }
    }
    
    public record LoginUserRequest(string Username, string Password);

    public record LoginUserResponse(string Token, Guid Id);
}