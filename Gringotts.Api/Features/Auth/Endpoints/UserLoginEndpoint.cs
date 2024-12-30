using Gringotts.Api.Features.Auth.Services;
using Gringotts.Api.Shared.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.Auth.Endpoints;

public class UserLoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup("/auth").MapPost("/login", HandleAsync);
    }

    private record LoginUserRequest(string Email, string Password);

    private record LoginUserResponse(string Token, Guid Id);

    private async Task<IResult> HandleAsync(
        [FromBody] LoginUserRequest request,
        UserSecretService userSecretService,
        UserJwtService userJwtService
    )
    {
        var passwordCheckResult = await userSecretService.MatchSecretAsync(request.Email, request.Password);

        if (!passwordCheckResult.IsSuccess) return Results.Unauthorized();

        var loggedInUserId = passwordCheckResult.Value;

        var token = userJwtService.GenerateUserToken(loggedInUserId, request.Email);

        return TypedResults.Ok(new LoginUserResponse(token, loggedInUserId));
    }
}