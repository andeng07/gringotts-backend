using Gringotts.Api.Features.Authentication.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.Authentication.Services;

public class UserSecretService(AppDbContext dbContext, HashingService hashingService)
{
    public async Task<TypedOperationResult<UserSecret>> FindSecret(string email)
    {
        var secret = await dbContext.UserSecrets.FirstOrDefaultAsync(secret => secret.Email == email);

        if (secret == null)
        {
            return TypedOperationResult<UserSecret>.Failure(new ErrorResponse("Secrets.User.SecretNotFound",
                "The secret with the email was not found.", ErrorResponse.ErrorType.NotFound));
        }

        return TypedOperationResult<UserSecret>.Success(secret);
    }    
    
    public async Task<TypedOperationResult<UserSecret>> FindSecret(Guid id)
    {
        var secret = await dbContext.UserSecrets.FirstOrDefaultAsync(secret => secret.UserId == id);

        if (secret == null)
        {
            return TypedOperationResult<UserSecret>.Failure(new ErrorResponse("Secrets.User.SecretNotFound",
                "The secret with the id was not found.", ErrorResponse.ErrorType.NotFound));
        }

        return TypedOperationResult<UserSecret>.Success(secret);
    }


    public async Task<TypedOperationResult<UserSecret>> CreateSecretAsync(Guid userId, string email, string password)
    {
        var secret = new UserSecret
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = email,
            Password = hashingService.Hash(password)
        };

        await dbContext.UserSecrets.AddAsync(secret);

        await dbContext.SaveChangesAsync();

        return secret;
    }

    public async Task<OperationResult> DeleteSecretAsync(Guid id)
    {
        var secret = await FindSecret(id);

        if (!secret.IsSuccess)
        {
            return OperationResult.Failure(secret.Errors.ToArray());
        }
        
        dbContext.UserSecrets.Remove(secret.Value!);
        await dbContext.SaveChangesAsync();

        return OperationResult.Success();
    }

    public async Task<bool> HasSecretAsync(string email) =>
        await dbContext.UserSecrets.AnyAsync(secret => secret.Email == email);

    public async Task<TypedOperationResult<Guid>> MatchSecretAsync(string email, string passwordAttempt)
    {
        
        var secret = await FindSecret(email);

        if (!secret.IsSuccess)
        {
            return TypedOperationResult<Guid>.Failure(secret.Errors.ToArray());
        }

        var result = hashingService.Verify(passwordAttempt, secret.Value!.Password);

        if (!result)
        {
            return TypedOperationResult<Guid>.Failure(new ErrorResponse(
                "Secrets.User.InvalidPassword", "The password is incorrect.", ErrorResponse.ErrorType.AccessUnauthorized));
        }

        return secret.Value.UserId;
    }
}