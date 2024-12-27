using Gringotts.Api.Features.Auth.Services;
using Gringotts.Api.Features.User.Services;
using Gringotts.Api.Shared.Endpoints;
using Gringotts.Api.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace Gringotts.Api.Features.User.Endpoints;

public class RegisterUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup("/user").MapPost("/register", HandleAsync);
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

    private async Task<TypedResult<RegisterUserResponse>> HandleAsync(
        [FromBody] RegisterUserRequest request,
        UserSecretService userSecretService,
        UserService userService,
        CancellationToken cancellationToken
    )
    {
        if (await userSecretService.HasSecretAsync(request.Email))
        {
            return TypedResult<RegisterUserResponse>.Failure(new Error(
                "Authentication.Register.EmailAlreadyRegistered",
                "An account with this email address already exists.",
                Error.ErrorType.Conflict
            ));
        }
        
        var registeredUserResult = await userService.CreateUserAsync(
            request.CardId,
            request.SchoolId,
            request.FirstName,
            request.LastName,
            request.MiddleName
        );

        if (!registeredUserResult.IsSuccess)
        {
            return TypedResult<RegisterUserResponse>.Failure(registeredUserResult.Errors.ToArray());
        }

        var createSecretResult = await userSecretService.CreateSecretAsync(
            registeredUserResult.Value!.Id,
            request.Email,
            request.Password
        );

        if (!createSecretResult.IsSuccess)
        {
            return TypedResult<RegisterUserResponse>.Failure(createSecretResult.Errors.ToArray());
        }
        
        var response = new RegisterUserResponse(registeredUserResult.Value!.Id, createSecretResult.Value!.Email);
        
        return TypedResult<RegisterUserResponse>.Success(response);
    }
}