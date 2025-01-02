using Gringotts.Api.Shared.Models;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Shared.Filters;

/// <summary>
/// An endpoint filter that checks whether an entity exists based on the ID provided in the route.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to check for existence. Must implement the <see cref="IEntity"/> interface.</typeparam>
public class EntityExistenceFromRoute<TEntity>(DbSet<TEntity> databaseSet, string idRouteKey)
    : IEndpointFilter where TEntity : class, IEntity
{
    /// <summary>
    /// Executes the filter logic to check if an entity exists based on the ID provided in the route.
    /// The ID is extracted from the route parameters and validated. If the ID is missing, invalid, 
    /// or the entity does not exist in the database, an appropriate error response is returned.
    /// </summary>
    /// <param name="context">The context for invoking the endpoint filter, providing access to the request data.</param>
    /// <param name="next">The delegate to invoke the next filter in the pipeline if the entity exists.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation of the filter. 
    /// If the entity does not exist or the ID is invalid, a BadRequest or NotFound response is returned.</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var idFromRoute = context.HttpContext.Request.RouteValues.TryGetValue(idRouteKey, out var idValue);

        if (!idFromRoute || idValue == null)
        {
            var error = new Error(
                "",
                "The ID parameter is required but was not provided. Please ensure that the ID is included in the request.",
                Error.ErrorType.Validation
            );

            return Microsoft.AspNetCore.Http.Results.BadRequest(error);
        }

        if (!Guid.TryParse(idValue.ToString(), out var id))
        {
            return Microsoft.AspNetCore.Http.Results.BadRequest("Invalid ID format.");
        }

        if (!await databaseSet.AnyAsync(x => x.Id == id))
        {
            var error = new List<Error>
            {
                new($"{typeof(TEntity).Name}.NotFound",
                    $"{typeof(TEntity).Name} not found.",
                    Error.ErrorType.NotFound)
            };
            return Microsoft.AspNetCore.Http.Results.NotFound(error);
        }

        return await next(context);
    }
}