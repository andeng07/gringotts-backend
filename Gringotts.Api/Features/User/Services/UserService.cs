using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.User.Services;

public class UserService(AppDbContext dbContext)
{
    public async Task<TypedOperationResult<User.Models.User>> GetLogUserByCardIdAsync(string cardId)
    {
        var logUser = await dbContext.Set<User.Models.User>().FirstOrDefaultAsync(x => x.CardId == cardId);

        if (logUser is null)
        {
            return new ErrorResponse(
                "USER_NOT_FOUND", 
                $"No user found with Card ID: {cardId}", 
                ErrorResponse.ErrorType.NotFound
            );
        }

        return logUser;
    }

}