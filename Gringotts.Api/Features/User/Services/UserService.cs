using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.User.Services;

public class UserService(AppDbContext dbContext)
{
    public async Task<TypedOperationResult<User.Models.User>> GetLogUserByCardIdAsync(string cardId)
    {
        var user = await dbContext.Set<User.Models.User>().FirstOrDefaultAsync(x => x.CardId == cardId);

        if (user == null)
        {
            var error = new ErrorResponse(
                "", // TODO
                "", // TODO
                ErrorResponse.ErrorType.NotFound
            );

            return error;
        }

        return user;
    }
}