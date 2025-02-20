using System.Security.Claims;
using Gringotts.Api.Shared.Results;

namespace Gringotts.Api.Shared.Filters.Authorization;

/// <summary>
/// A filter that ensures the entity being accessed is owned by the authenticated user.
/// This filter checks the entity ID from the route and ensures it matches the authenticated user's ID.
/// </summary>
/// <param name="idRouteKey">The key used to extract the ID from the route parameters.</param>
public class EntityOwnershipFromRouteFilter(string idRouteKey) : IEndpointFilter
{
    
    /// <summary>
    /// Applies the ownership check to the request. 
    /// It validates that the ID exists in the route, is in a valid GUID format, and belongs to the authenticated user.
    /// </summary>
    /// <param name="context">The context of the current request, including HTTP context and route data.</param>
    /// <param name="next">The delegate representing the next filter or endpoint handler in the pipeline.</param>
    /// <returns>A task that represents the asynchronous operation. The result is the response to be returned.</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var idFromRoute = context.HttpContext.Request.RouteValues.TryGetValue(idRouteKey, out var idValue);

        if (!idFromRoute || idValue == null)
        {
            var error = new ErrorResponse(
                "",
                "The ID parameter is required but was not provided. Please ensure that the ID is included in the request.",
                ErrorResponse.ErrorType.Validation
            );

            return Microsoft.AspNetCore.Http.Results.BadRequest(error);
        }

        if (!Guid.TryParse(idValue.ToString(), out var id))
        {
            return Microsoft.AspNetCore.Http.Results.BadRequest("Invalid ID format.");
        }

        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Microsoft.AspNetCore.Http.Results.Forbid();
        }

        var userIdAsGuid = Guid.Parse(userId);

        if (userIdAsGuid != id)
        {
            return Microsoft.AspNetCore.Http.Results.Forbid();
        }

        return await next(context);
    }
}