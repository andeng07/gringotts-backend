using Gringotts.Api.Features.ManagementAuthentication.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.ManagementAuthentication.Services;

public class ManagementUserSecretService(AppDbContext dbContext, HashingService hashingService)
{
    public async Task<TypedOperationResult<ManagementUserSecret>> FindSecret(string username)
    {
        var secret = await dbContext.ManagementUserSecrets.FirstOrDefaultAsync(secret => secret.Username == username);

        if (secret == null)
        {
            return TypedOperationResult<ManagementUserSecret>.Failure(new ErrorResponse("Secrets.LogUser.SecretNotFound",
                "The secret with the username was not found.", ErrorResponse.ErrorType.NotFound));
        }

        return TypedOperationResult<ManagementUserSecret>.Success(secret);
    }    
    
    public async Task<TypedOperationResult<ManagementUserSecret>> FindSecret(Guid id)
    {
        var secret = await dbContext.ManagementUserSecrets.FirstOrDefaultAsync(secret => secret.ManagementUserId == id);

        if (secret == null)
        {
            return TypedOperationResult<ManagementUserSecret>.Failure(new ErrorResponse("Secrets.LogUser.SecretNotFound",
                "The secret with the id was not found.", ErrorResponse.ErrorType.NotFound));
        }

        return TypedOperationResult<ManagementUserSecret>.Success(secret);
    }


    public async Task<TypedOperationResult<ManagementUserSecret>> CreateSecretAsync(Guid userId, string username, string password)
    {
        var secret = new ManagementUserSecret
        {
            Id = Guid.NewGuid(),
            ManagementUserId = userId,
            Username = username,
            Password = hashingService.Hash(password)
        };

        await dbContext.ManagementUserSecrets.AddAsync(secret);

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
        
        dbContext.ManagementUserSecrets.Remove(secret.Value!);
        await dbContext.SaveChangesAsync();

        return OperationResult.Success();
    }

    public async Task<bool> HasSecretAsync(string username) =>
        await dbContext.ManagementUserSecrets.AnyAsync(secret => secret.Username == username);

    public async Task<TypedOperationResult<Guid>> MatchSecretAsync(string username, string passwordAttempt)
    {
        
        var secret = await FindSecret(username);

        if (!secret.IsSuccess)
        {
            return TypedOperationResult<Guid>.Failure(secret.Errors.ToArray());
        }

        var result = hashingService.Verify(passwordAttempt, secret.Value!.Password);

        if (!result)
        {
            return TypedOperationResult<Guid>.Failure(new ErrorResponse(
                "Secrets.LogUser.InvalidPassword", "The password is incorrect.", ErrorResponse.ErrorType.AccessUnauthorized));
        }

        return secret.Value.ManagementUserId;
    }
}