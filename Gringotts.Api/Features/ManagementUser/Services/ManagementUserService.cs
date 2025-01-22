using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.ManagementUser.Services;

public class ManagementUserService(AppDbContext dbContext)
{
    public async Task<TypedOperationResult<Models.ManagementUser>> CreateUserAsync(string firstName, string lastName,
        string? middleName = null)
    {
        var userId = Guid.NewGuid();

        var newUser = new Models.ManagementUser
        {
            Id = userId,
            FirstName = firstName,
            LastName = lastName,
            MiddleName = middleName
        };

        await dbContext.ManagementUsers.AddAsync(newUser);

        await dbContext.SaveChangesAsync();

        return newUser;
    }

    public async Task<TypedOperationResult<Models.ManagementUser>> GetUserByIdAsync(Guid userId)
    {
        var user = await dbContext.ManagementUsers.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return new ErrorResponse(UserErrorCodes.UserNotFound, $"LogUser with id \"{userId}\" not found",
                ErrorResponse.ErrorType.NotFound);
        }

        return user;
    }

    public async Task<bool> IsUserRegistered(Guid id) => await dbContext.ManagementUserSecrets.AnyAsync(x => x.Id == id);
}