using Gringotts.Api.Features.UserSecrets.Services;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.UserSecrets.Endpoints;

public class RegisterUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup("/user-auth").MapPost("/register-user", HandleAsync)
            .AddEndpointFilter<ValidationEndpointFilter<RegisterUserRequest>>();
    }

    private record struct RegisterUserRequest(string Email, string Password);

    private record struct RegisterUserResponse(Guid UserId);

    private static async Task<TypedResult<RegisterUserResponse>> HandleAsync(
        [FromBody] RegisterUserRequest request,
        UserSecretService userSecretService,
        UserJwtService userJwtService,
        CancellationToken cancellationToken
    )
    {
        if (await userSecretService.HasSecret(request.Email))
        {
            return TypedResult<RegisterUserResponse>.Failure(new Error(
                "Authentication.Register.EmailAlreadyRegistered",
                "An account with this email address already exists.",
                Error.ErrorType.Conflict
            ));
        }
        
        

        /* TODO:
         * 1. Valid user existence
         * 2. Create user
         * 3. Respond with ID
         */

        return TypedResult<RegisterUserResponse>.Success(new RegisterUserResponse(Guid.NewGuid()));
    }
}