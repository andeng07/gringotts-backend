using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Features.LogUser.Services;

public class LogUserService(AppDbContext dbContext)
{
    public async Task<TypedOperationResult<Models.LogUser>> GetLogUserByCardIdAsync(string cardId)
    {
        var user = await dbContext.Set<Models.LogUser>().FirstOrDefaultAsync(x => x.CardId == cardId);

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