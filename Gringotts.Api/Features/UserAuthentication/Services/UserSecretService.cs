using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Database.Models.Users;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.UserAuthentication.Services;

public class UserSecretService(AppDbContext dbContext, HashService hashService)
{
    public async Task<TypedResult<UserSecret>> FindSecret(string email)
    {
        var secret = await dbContext.UserSecrets.FirstOrDefaultAsync(secret => secret.Email == email);

        if (secret == null)
        {
            return TypedResult<UserSecret>.Failure(new Error("Secrets.User.SecretNotFound",
                "The secret with the email was not found.", Error.ErrorType.NotFound));
        }

        return TypedResult<UserSecret>.Success(secret);
    }    
    
    public async Task<TypedResult<UserSecret>> FindSecret(Guid id)
    {
        var secret = await dbContext.UserSecrets.FirstOrDefaultAsync(secret => secret.UserId == id);

        if (secret == null)
        {
            return TypedResult<UserSecret>.Failure(new Error("Secrets.User.SecretNotFound",
                "The secret with the id was not found.", Error.ErrorType.NotFound));
        }

        return TypedResult<UserSecret>.Success(secret);
    }


    public async Task<TypedResult<Guid>> CreateSecretAsync(string email, string password)
    {
        if (await HasSecretAsync(email))
        {
            return TypedResult<Guid>.Failure(Error.Failure("Secrets.User.SecretWithEmailExists",
                "The secret with the email already exists."));
        }

        var id = Guid.NewGuid();

        var secret = new UserSecret
        {
            UserId = Guid.NewGuid(),
            Id = id,
            Email = email,
            Password = hashService.Hash(password)
        };

        await dbContext.UserSecrets.AddAsync(secret);

        return TypedResult<Guid>.Success(id);
    }

    public async Task<Result> DeleteSecretAsync(Guid id)
    {
        var secret = await FindSecret(id);

        if (!secret.IsSuccess)
        {
            return Result.Failure(secret.Errors.ToArray());
        }
        
        dbContext.UserSecrets.Remove(secret.Value!);
        await dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<bool> HasSecretAsync(string email) =>
        await dbContext.UserSecrets.AnyAsync(secret => secret.Email == email);

    public async Task<TypedResult<Guid>> MatchSecretAsync(string email, string passwordAttempt)
    {
        
        var secret = await FindSecret(email);

        if (!secret.IsSuccess)
        {
            return TypedResult<Guid>.Failure(secret.Errors.ToArray());
        }

        var result = hashService.Verify(passwordAttempt, secret.Value!.Password);

        if (!result)
        {
            return TypedResult<Guid>.Failure(new Error(
                "Secrets.User.InvalidPassword", "The password is incorrect.", Error.ErrorType.AccessUnauthorized));
        }

        return secret.Value.UserId;
    }
}