using System.Security.Claims;
using Gringotts.Api.Shared.Results;
using Gringotts.Api.Shared.Services;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Gringotts.Api.Features.UserAuthentication.Services;

public class UserJwtService(IConfiguration configuration) : JwtService(configuration)
{
    
    public string GenerateUserToken(string userId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new("TokenType", "User"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        return GenerateToken(claims);
    }

    public TypedResult<ClaimsPrincipal> ValidateUserToken(string token)
    {
        return ValidateToken(token, false);
    }
    
}