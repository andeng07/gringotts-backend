using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gringotts.Api.Shared.Exceptions;
using Gringotts.Api.Shared.Results;
using Microsoft.IdentityModel.Tokens;

namespace Gringotts.Api.Shared.Services;

/// <summary>
/// An abstract class providing functionality for generating and validating JSON Web Tokens (JWT).
/// It handles JWT creation with claims and secret keys as well as validating the token's authenticity and expiration.
/// </summary>
/// <param name="configuration">The configuration containing JWT settings such as secret, issuer, and audience.</param>
public abstract class JwtService(IConfiguration configuration)
{
    private readonly string _secret = configuration["Jwt:Secret"] ?? throw new ConfigurationMissingFieldException("Jwt:Secret");
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? throw new ConfigurationMissingFieldException("Jwt:Issuer");
    private readonly string _audience = configuration["Jwt:Audience"] ?? throw new ConfigurationMissingFieldException("Jwt:Audience");
    
    /// <summary>
    /// Generates a JWT token based on the specified claims and optional expiration time.
    /// </summary>
    /// <param name="claims">The claims to be included in the JWT token.</param>
    /// <param name="expires">Optional expiration date for the token. If not provided, the token will not expire.</param>
    /// <returns>A string representing the generated JWT token.</returns>
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

    /// <summary>
    /// Validates the provided JWT token and returns the claims principal if valid.
    /// </summary>
    /// <param name="token">The JWT token to be validated.</param>
    /// <param name="validateLifetime">A flag indicating whether to validate the token's expiration time.</param>
    /// <returns>A <see cref="TypedResult{ClaimsPrincipal}"/> containing the validated claims principal or an error result if validation fails.</returns>
    protected TypedResult<ClaimsPrincipal> ValidateToken(string token, bool validateLifetime)
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
        catch(Exception e)
        {
            var error = new Error("Jwt.Token.ValidationFailure", e.Message, Error.ErrorType.Failure);
            return TypedResult<ClaimsPrincipal>.Failure(error);
        }
    }
    
}