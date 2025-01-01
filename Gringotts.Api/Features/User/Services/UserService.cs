using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.User.Services;

public class UserService(AppDbContext dbContext)
{
    // TODO: finalize the data being stored
    public async Task<TypedResult<Models.User>> CreateUserAsync(string cardId, string schoolId, string firstName, string lastName, string? middleName = null)
    {
        var userId = Guid.NewGuid();

        var newUser = new Models.User
        {
            Id = userId,
            CardId = cardId,
            SchoolId = schoolId,
            FirstName = firstName,
            LastName = lastName,
            MiddleName = middleName
        };
        
        await dbContext.Users.AddAsync(newUser);

        await dbContext.SaveChangesAsync();
        
        return newUser;
    }

    public async Task<TypedResult<Models.User>> GetUserByIdAsync(Guid userId)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return new Error(UserErrorCodes.UserNotFound, $"User with id \"{userId}\" not found", Error.ErrorType.NotFound);
        }

        return user;
    }
        
    public async Task<bool> IsUserRegistered(Guid id) => await dbContext.Users.AnyAsync(x => x.Id == id);
}