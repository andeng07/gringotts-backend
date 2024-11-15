using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Database.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Shared.Services;

public class UserService(AppDbContext dbContext)
{
    
    public async Task<bool> IsUserRegistered(Guid id) => await dbContext.Users.AnyAsync(x => x.Id == id);
    
}