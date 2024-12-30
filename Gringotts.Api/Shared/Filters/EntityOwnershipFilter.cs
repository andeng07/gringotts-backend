using System.Security.Claims;

namespace Gringotts.Api.Shared.Filters;

/// <summary>
/// An endpoint filter that checks if the entity is owned by the current user based on the entity ID extracted
/// from the request and the user ID from the authentication claims.
/// </summary>
/// <typeparam name="TRequest">The type of the request object containing the entity ID.</typeparam>
/// <param name="idSelector">A function that selects the entity ID from the request object.</param>
public class EntityOwnershipFilter<TRequest>(Func<TRequest, Guid> idSelector)
    : IEndpointFilter
{
    /// <summary>
    /// Executes the filter logic to check if the current user owns the entity.
    /// The ownership check is performed by comparing the user ID (from the authentication claims) 
    /// with the entity ID extracted from the request.
    /// If the user is not authenticated or does not own the entity, an appropriate error response is returned.
    /// </summary>
    /// <param name="context">The context for invoking the endpoint filter.</param>
    /// <param name="next">The delegate to invoke the next filter in the pipeline.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation of the filter.</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().Single();

        var id = idSelector(request);

        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Microsoft.AspNetCore.Http.Results.BadRequest(
                "The user identifier (UserId) is missing from the authentication claims. Please ensure the user is properly authenticated."
            );
        }
        
        var userIdAsGuid = Guid.Parse(userId);

        if (userIdAsGuid != id)
        {
            return Microsoft.AspNetCore.Http.Results.Unauthorized();
        }
        
        return await next(context);
    }
}