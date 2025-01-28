using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.Client.Services;

public class ClientService(AppDbContext dbContext)
{
    public async Task<TypedOperationResult<Client.Models.Client>> CreateUserAsync(string firstName, string lastName,
        string? middleName = null)
    {
        var userId = Guid.NewGuid();

        var newUser = new Client.Models.Client
        {
            Id = userId,
            FirstName = firstName,
            LastName = lastName,
            MiddleName = middleName
        };

        await dbContext.Clients.AddAsync(newUser);

        await dbContext.SaveChangesAsync();

        return newUser;
    }

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

    public async Task<bool> IsUserRegistered(Guid id) => await dbContext.ClientSecrets.AnyAsync(x => x.Id == id);
}