using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gringotts.Api.Shared.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace Gringotts.Api.Shared.Services;

public abstract class JwtService(IConfiguration configuration)
{
    private readonly string _secret = configuration["Jwt:Secret"] ?? throw new ConfigurationMissingFieldException("Jwt:Secret");
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? throw new ConfigurationMissingFieldException("Jwt:Issuer");
    private readonly string _audience = configuration["Jwt:Audience"] ?? throw new ConfigurationMissingFieldException("Jwt:Audience");
    
    protected string GenerateToken(IEnumerable<Claim> claims, DateTime? expires = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    protected ClaimsPrincipal ValidateToken(string token, bool validateLifetime)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = validateLifetime,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null; // or throw an exception based on your needs
        }
    }
    
}