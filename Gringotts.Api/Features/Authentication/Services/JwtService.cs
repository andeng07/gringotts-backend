using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gringotts.Api.Shared.Errors;
using Gringotts.Api.Shared.Results;
using Microsoft.IdentityModel.Tokens;

namespace Gringotts.Api.Features.Authentication.Services;

/// <summary>
/// A service for generating and validating JSON Web Tokens (JWT).
/// </summary>
public class JwtService(IConfiguration configuration)
{
    private readonly string _secret =
        configuration["Jwt:Secret"] ?? throw new ConfigurationMissingFieldException("Jwt:Secret");

    private readonly string _issuer =
        configuration["Jwt:Issuer"] ?? throw new ConfigurationMissingFieldException("Jwt:Issuer");

    private readonly string _audience =
        configuration["Jwt:Audience"] ?? throw new ConfigurationMissingFieldException("Jwt:Audience");
    
    /// <summary>
    /// Generates a JWT token based on the specified claims and optional expiration time.
    /// </summary>
    /// <param name="claims">The claims to be included in the JWT token.</param>
    /// <param name="expires">Optional expiration date for the token. If not provided, the token will not expire.</param>
    /// <returns>A string representing the generated JWT token.</returns>
    public string GenerateToken(IEnumerable<Claim> claims, DateTime? expires = null)
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
    /// Generates a user-specific JWT token.
    /// </summary>
    /// <param name="subjectId">The ID of the subject.</param>
    /// <returns>A JWT token containing user-specific claims.</returns>
    public string GenerateToken(Guid subjectId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, subjectId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        return GenerateToken(claims);
    }

    /// <summary>
    /// Validates the provided JWT token and returns the claims principal if valid.
    /// </summary>
    /// <param name="token">The JWT token to be validated.</param>
    /// <param name="validateLifetime">A flag indicating whether to validate the token's expiration time.</param>
    /// <returns>A <see cref="TypedOperationResult{TValue}"/> containing the validated claims principal or an error operationResult if validation fails.</returns>
    public TypedOperationResult<ClaimsPrincipal> ValidateToken(string token, bool validateLifetime)
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
        catch (Exception e)
        {
            var error = new ErrorResponse("Jwt.Token.ValidationFailure", e.Message, ErrorResponse.ErrorType.Failure);
            return TypedOperationResult<ClaimsPrincipal>.Failure(error);
        }
    }
    
}