using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.User.Services;

public class UserService(AppDbContext dbContext)
{
    // TODO: finalize the data being stored
    public async Task<TypedResult<Guid>> CreateUser(string cardId, string schoolId, string firstName, string lastName, string? middleName = null)
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
        
        dbContext.Users.Add(newUser);

        await dbContext.SaveChangesAsync();

        return userId;
    }
        
    public async Task<bool> IsUserRegistered(Guid id) => await dbContext.Users.AnyAsync(x => x.Id == id);
}