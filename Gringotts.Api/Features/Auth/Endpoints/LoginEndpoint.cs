using Gringotts.Api.Features.Auth.Services;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Auth.Endpoints;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup("/user/auth").MapPost("/login", HandleAsync)
            .AddEndpointFilter<ValidationEndpointFilter<LoginUserRequest>>();
    }

    private record LoginUserRequest(string Email, string Password);

    private record LoginUserResponse(string Token, Guid Id);

    private async Task<TypedResult<LoginUserResponse>> HandleAsync(
        [FromBody] LoginUserRequest request,
        UserSecretService userSecretService,
        UserJwtService userJwtService
    )
    {
        var passwordCheckResult = await userSecretService.MatchSecretAsync(request.Email, request.Password);

        if (!passwordCheckResult.IsSuccess)
        {
            // Generate a generic error message to prevent revealing details like incorrect email or password for security reasons.
            
            return TypedResult<LoginUserResponse>.Failure(
                new Error("Authentication.Login.InvalidCredentials",
                    "The username or email provided does not match any existing account in the system",
                    Error.ErrorType.AccessUnauthorized)
            );
        }

        var loggedInUserId = passwordCheckResult.Value;
        
        var token = userJwtService.GenerateUserToken(loggedInUserId);
        
        return TypedResult<LoginUserResponse>.Success(new LoginUserResponse(token, loggedInUserId));
    }
}