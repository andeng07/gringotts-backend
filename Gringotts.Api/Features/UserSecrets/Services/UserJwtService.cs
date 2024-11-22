using System.Security.Claims;
using Gringotts.Api.Shared.Services;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Gringotts.Api.Features.UserSecrets.Services;

public class UserJwtService(IConfiguration configuration) : JwtService(configuration)
{
    
    public string GenerateUserToken(string userId, string username)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new("TokenType", "User"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        return GenerateToken(claims);
    }

    public ClaimsPrincipal ValidateUserToken(string token)
    {
        return ValidateToken(token, false);
    }
    
}