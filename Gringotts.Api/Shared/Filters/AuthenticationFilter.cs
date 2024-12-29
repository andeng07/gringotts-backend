using Gringotts.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Gringotts.Api.Shared.Filters;

/// <summary>
/// An endpoint filter that performs authentication by validating the provided JWT token.
/// It checks for the presence of the "Authorization" header in the HTTP request and validates
/// the token using the provided <see cref="UserJwtService"/>.
/// </summary>
/// <param name="userJwtService">The service used to validate the JWT token.</param>
public class AuthenticationFilter(UserJwtService userJwtService) : IEndpointFilter
{
    /// <summary>
    /// Invokes the authentication logic and checks the validity of the JWT token.
    /// If the token is missing, invalid, or expired, the request will be rejected with a 401 Unauthorized response.
    /// </summary>
    /// <param name="context">The context of the current request.</param>
    /// <param name="next">The delegate to invoke the next filter in the pipeline.</param>
    /// <returns>An <see cref="IResult"/> representing the result of the filter invocation.</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        string? authorizationHeader = context.HttpContext.Request.Headers.Authorization;

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return Microsoft.AspNetCore.Http.Results.Unauthorized();
        }
        
        var token = authorizationHeader["Bearer ".Length..].Trim();
        
        var validateUserTokenResult = userJwtService.ValidateUserToken(token);
        
        if (!validateUserTokenResult.IsSuccess)
        {
            return Microsoft.AspNetCore.Http.Results.Unauthorized();
        }
        
        return await next(context);
    }
    
}