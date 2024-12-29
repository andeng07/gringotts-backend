using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Filters;
using Gringotts.Api.Shared.Models;

namespace Gringotts.Api.Shared.Extensions;

/// <summary>
/// Extension methods for adding filters to route handlers.
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    /// Adds an entity ownership filter to the route handler pipeline. This filter ensures that the entity
    /// is owned by the current user (as determined by the provided <paramref name="idSelector"/> function).
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object.</typeparam>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to which the filter is added.</param>
    /// <param name="idSelector">A function that selects the entity ID from the request object.</param>
    /// <returns>The modified <see cref="RouteHandlerBuilder"/> with the entity ownership filter.</returns>
    public static RouteHandlerBuilder WithEntityOwnershipFilter<TRequest>(this RouteHandlerBuilder builder,
        Func<TRequest, Guid> idSelector)
    {
        return builder.AddEndpointFilterFactory(
            (_, next) => async context =>
            {
                var filter = new EntityOwnershipFilter<TRequest>(idSelector);
                return await filter.InvokeAsync(context, next);
            }
        );
    }

    /// <summary>
    /// Adds an entity existence filter to the route handler pipeline. This filter ensures that the entity
    /// exists in the database (as determined by the provided <paramref name="idSelector"/> function).
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity class that must exist in the database.</typeparam>
    /// <typeparam name="TRequest">The type of the request object.</typeparam>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to which the filter is added.</param>
    /// <param name="idSelector">A function that selects the entity ID from the request object.</param>
    /// <returns>The modified <see cref="RouteHandlerBuilder"/> with the entity existence filter.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the database set or the entity does not exist.</exception>
    public static RouteHandlerBuilder WithEntityExistenceFilter<TEntity, TRequest>(this RouteHandlerBuilder builder,
        Func<TRequest, Guid> idSelector) where TEntity : class, IEntity
    {
        return builder.AddEndpointFilterFactory((_, next) => async context =>
        {
            var databaseSet = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>().Set<TEntity>();
            var filter = new EntityExistenceFilter<TEntity, TRequest>(databaseSet, idSelector);
            return await filter.InvokeAsync(context, next);
        });
    }

    public static RouteHandlerBuilder WithAuthenticationFilter(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<AuthenticationFilter>();
    }

}
