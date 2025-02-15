using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.Client.Services;

/// <summary>
/// Provides operations for managing clients.
/// </summary>
/// <remarks>
/// This service handles client-related operations such as creation, retrieval, 
/// and validation of client existence within the system.
/// </remarks>
public class ClientService(AppDbContext dbContext)
{
    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="firstName">The first name of the client.</param>
    /// <param name="lastName">The last name of the client.</param>
    /// <param name="middleName">The middle name of the client (optional).</param>
    /// <returns>
    /// A <see cref="TypedOperationResult{T}"/> containing the created client details.
    /// </returns>
    public async Task<TypedOperationResult<Client.Models.Client>> CreateUserAsync(string firstName, string lastName,
        string? middleName = null)
    {
        var userId = Guid.NewGuid();

        var newUser = new Client.Models.Client
        {
            Id = userId,
            CreatedAt = DateTime.UtcNow,
            FirstName = firstName,
            LastName = lastName,
            MiddleName = middleName
        };

        await dbContext.Clients.AddAsync(newUser);

        await dbContext.SaveChangesAsync();

        return newUser;
    }

    /// <summary>
    /// Retrieves a client by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the client.</param>
    /// <returns>
    /// A <see cref="TypedOperationResult{T}"/> containing the client details if found,
    /// otherwise an error response indicating the client was not found.
    /// </returns>
    public async Task<TypedOperationResult<Client.Models.Client>> GetUserByIdAsync(Guid userId)
    {
        var user = await dbContext.Clients.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return new ErrorResponse(UserErrorCodes.UserNotFound, $"User with id \"{userId}\" not found",
                ErrorResponse.ErrorType.NotFound);
        }

        return user;
    }

    /// <summary>
    /// Checks if a client is registered in the system.
    /// </summary>
    /// <param name="id">The unique identifier of the client.</param>
    /// <returns>
    /// A boolean indicating whether the client exists.
    /// </returns>
    public async Task<bool> IsUserRegistered(Guid id) => await dbContext.ClientSecrets.AnyAsync(x => x.Id == id);
}