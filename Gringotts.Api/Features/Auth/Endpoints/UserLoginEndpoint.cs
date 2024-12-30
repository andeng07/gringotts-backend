﻿using FluentValidation;
using Gringotts.Api.Features.Auth.Services;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Extensions;
using Gringotts.Api.Shared.Results;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Auth.Endpoints;

/// <summary>
/// Endpoint for handling user login.
/// </summary>
/// <remarks>
/// This endpoint validates user credentials (email and password) and returns a JWT token if the login is successful.
/// If the login fails, it returns a 401 Unauthorized error. Additionally, validation errors are handled with appropriate error codes.
/// </remarks>
/// <response code="200">Returns a JWT token and the user's ID when login is successful.</response>
/// <response code="401">Returns an error if the email or password is incorrect.</response>
public class UserLoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup("/auth").MapPost("/login", HandleAsync)
            .WithRequestValidation<LoginRequest>()
            .Produces<LoginUserResponse>()
            .Produces<List<Error>>(StatusCodes.Status401Unauthorized);
    }
    
    private static async Task<IResult> HandleAsync(
        [FromBody] LoginUserRequest request,
        UserSecretService userSecretService,
        JwtService jwtService
    )
    {
        var passwordCheckResult = await userSecretService.MatchSecretAsync(request.Email, request.Password);

        if (!passwordCheckResult.IsSuccess)
        {
            var errors = new List<Error>
            {
                new(
                    AuthErrorCodes.CredentialsInvalid,
                    "Invalid email or password.",
                    Error.ErrorType.Validation
                )
            };
            return Results.Json(errors, statusCode: StatusCodes.Status401Unauthorized);
        }

        var loggedInUserId = passwordCheckResult.Value;
        var token = jwtService.GenerateToken(loggedInUserId);

        return Results.Ok(new LoginUserResponse(token, loggedInUserId));
    }
    
    private class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.").WithErrorCode(ValidationErrorCodes.EmailRequired)
                .EmailAddress().WithMessage("Invalid email format.").WithErrorCode(ValidationErrorCodes.InvalidEmailFormat);
            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.").WithErrorCode(ValidationErrorCodes.PasswordRequired)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.").WithErrorCode(ValidationErrorCodes.PasswordTooShort);
        }
    }
    
    private record LoginUserRequest(string Email, string Password);

    private record LoginUserResponse(string Token, Guid Id);
}