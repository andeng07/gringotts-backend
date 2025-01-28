using Gringotts.Api.Features.ClientAuthentication.Services;

namespace Gringotts.Api.Shared.Filters.Authentication;

/// <summary>
/// An endpoint filter that performs authentication by validating the provided JWT token.
/// It checks for the presence of the "Authorization" header in the HTTP request and validates
/// the token using the provided <see cref="JwtService"/>.
/// </summary>
/// <param name="jwtService">The service used to validate the JWT token.</param>
public class AuthenticationFilter(JwtService jwtService) : IEndpointFilter
{
    /// <summary>
    /// Invokes the authentication logic to validate the JWT token.
    /// </summary>
    /// <param name="context">The context of the current request.</param>
    /// <param name="next">The delegate to invoke the next filter in the pipeline.</param>
    /// <returns>An <see cref="IResult"/> representing the result of the filter invocation.</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        string? authorizationHeader = context.HttpContext.Request.Headers.Authorization;

        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return Microsoft.AspNetCore.Http.Results.Unauthorized();
        }

        // Extract the token, ignoring any scheme or prefix
        var token = authorizationHeader.Contains(' ') ? authorizationHeader.Split(' ')[1] : authorizationHeader.Trim();

        // Validate the token
        var validationResult = jwtService.ValidateToken(token, false);

        if (!validationResult.IsSuccess)
        {
            return Microsoft.AspNetCore.Http.Results.Unauthorized();
        }

        // Attach the validated claims to the HttpContext for downstream use
        context.HttpContext.User = validationResult.Value!;

        return await next(context);
    }
    
}