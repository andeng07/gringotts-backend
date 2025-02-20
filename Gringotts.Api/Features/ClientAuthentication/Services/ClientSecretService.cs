using Gringotts.Api.Features.ClientAuthentication.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.ClientAuthentication.Services;

/// <summary>
/// Service for managing client secrets, including retrieval, creation, deletion, and authentication.
/// </summary>
public class ClientSecretService(AppDbContext dbContext, HashingService hashingService)
{
    /// <summary>
    /// Retrieves a client secret by username.
    /// </summary>
    /// <param name="username">The username associated with the secret.</param>
    /// <returns>A <see cref="TypedOperationResult{TValue}"/> containing the client secret if found, otherwise an error response.</returns>
    public async Task<TypedOperationResult<ClientSecret>> FindSecret(string username)
    {
        var secret = await dbContext.ClientSecrets.FirstOrDefaultAsync(secret => secret.Username.ToLower() == username.ToLower());

        if (secret == null)
        {
            return TypedOperationResult<ClientSecret>.Failure(new ErrorResponse(
                "Secrets.Client.SecretNotFoundByUsername",
                "No secret found for the given username.",
                ErrorResponse.ErrorType.NotFound));
        }

        return TypedOperationResult<ClientSecret>.Success(secret);
    }    

    /// <summary>
    /// Retrieves a client secret by the client ID.
    /// </summary>
    /// <param name="id">The unique identifier of the client.</param>
    /// <returns>A <see cref="TypedOperationResult{T}"/> containing the client secret if found, otherwise an error response.</returns>
    public async Task<TypedOperationResult<ClientSecret>> FindSecret(Guid id)
    {
        var secret = await dbContext.ClientSecrets.FirstOrDefaultAsync(secret => secret.ClientId == id);

        if (secret == null)
        {
            return TypedOperationResult<ClientSecret>.Failure(new ErrorResponse(
                "Secrets.Client.SecretNotFoundById",
                "No secret found for the given ID.",
                ErrorResponse.ErrorType.NotFound));
        }

        return TypedOperationResult<ClientSecret>.Success(secret);
    }

    /// <summary>
    /// Creates a new client secret for a client.
    /// </summary>
    /// <param name="userId">The unique identifier of the client.</param>
    /// <param name="username">The username associated with the secret.</param>
    /// <param name="password">The plaintext password to be hashed and stored.</param>
    /// <returns>A <see cref="TypedOperationResult{T}"/> containing the newly created client secret.</returns>
    public async Task<TypedOperationResult<ClientSecret>> CreateSecretAsync(Guid userId, string username, string password)
    {
        var secret = new ClientSecret
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            ClientId = userId,
            Username = username,
            Password = hashingService.Hash(password)
        };

        await dbContext.ClientSecrets.AddAsync(secret);
        await dbContext.SaveChangesAsync();

        return TypedOperationResult<ClientSecret>.Success(secret);
    }

    /// <summary>
    /// Deletes a client secret by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the client secret.</param>
    /// <returns>An <see cref="OperationResult"/> indicating success or failure.</returns>
    public async Task<OperationResult> DeleteSecretAsync(Guid id)
    {
        var secretResult = await FindSecret(id);

        if (!secretResult.IsSuccess)
        {
            return OperationResult.Failure(secretResult.Errors.ToArray());
        }

        dbContext.ClientSecrets.Remove(secretResult.Value!);
        await dbContext.SaveChangesAsync();

        return OperationResult.Success();
    }

    /// <summary>
    /// Checks if a client secret exists for a given username.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>A boolean indicating whether a secret exists.</returns>
    public async Task<bool> HasSecretAsync(string username) =>
        await dbContext.ClientSecrets.AnyAsync(secret => secret.Username == username);

    /// <summary>
    /// Matches a username and password attempt against stored secrets.
    /// </summary>
    /// <param name="username">The username of the secret.</param>
    /// <param name="passwordAttempt">The password attempt to verify.</param>
    /// <returns>A <see cref="TypedOperationResult{T}"/> containing the client ID if authentication is successful, otherwise an error response.</returns>
    public async Task<TypedOperationResult<Guid>> MatchSecretAsync(string username, string passwordAttempt)
    {
        var secretResult = await FindSecret(username);

        if (!secretResult.IsSuccess)
        {
            return TypedOperationResult<Guid>.Failure(secretResult.Errors.ToArray());
        }

        var secret = secretResult.Value!;
        var isPasswordValid = hashingService.Verify(passwordAttempt, secret.Password);

        if (!isPasswordValid)
        {
            return TypedOperationResult<Guid>.Failure(new ErrorResponse(
                "Secrets.Client.InvalidPassword",
                "Incorrect password.",
                ErrorResponse.ErrorType.AccessUnauthorized));
        }

        return TypedOperationResult<Guid>.Success(secret.ClientId);
    }
}