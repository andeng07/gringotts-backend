using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Database.Models.Users;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.UserSecrets.Services;

public class UserSecretService(AppDbContext dbContext)
{
    public async Task<TypedResult<Guid>> CreateAsync(string email, string password)
    {
        if (await HasSecret(email))
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
            Password = password
        };

        await dbContext.UserSecrets.AddAsync(secret);

        return TypedResult<Guid>.Success(id);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var secret = await dbContext.UserSecrets.FirstOrDefaultAsync(x => x.Id == id);

        if (secret == null)
        {
            return Result.Failure(new Error("Secrets.User.SecretNotFound", "The secret with the email was not found.",
                Error.ErrorType.NotFound));
        }

        dbContext.UserSecrets.Remove(secret);
        await dbContext.SaveChangesAsync();
        
        return Result.Success();
    }

    public async Task<bool> SecretsMatch(string email, string passwordAttempt)
    {
        // TODO
    }

    public async Task<bool> HasSecret(string email) =>
        await dbContext.UserSecrets.AnyAsync(secret => secret.Email == email);
}