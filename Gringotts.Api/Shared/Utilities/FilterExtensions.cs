using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Filters.Authentication;
using Gringotts.Api.Shared.Filters.Authorization;
using Gringotts.Api.Shared.Filters.Existence;
using Gringotts.Api.Shared.Filters.Validation;
using Gringotts.Api.Shared.Results;

namespace Gringotts.Api.Shared.Utilities;

/// <summary>
/// Extension methods for adding filters to route handlers.
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    /// Adds a request validation filter to the route handler pipeline. This filter validates the incoming
    /// request based on the specified request type.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object to be validated.</typeparam>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to which the validation filter is added.</param>
    /// <returns>The modified <see cref="RouteHandlerBuilder"/> with the request validation filter.</returns>
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .Produces(StatusCodes.Status400BadRequest);
    }

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
                })
            .Produces(StatusCodes.Status403Forbidden);
    }
    
    /// <summary>
    /// Adds an entity ownership filter to the route handler pipeline, where the entity ID is extracted
    /// directly from the route parameters. This filter ensures that the entity is owned by the current user.
    /// </summary>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to which the filter is added.</param>
    /// <param name="idRouteKey">The key used to extract the entity ID from the route.</param>
    /// <returns>The modified <see cref="RouteHandlerBuilder"/> with the entity ownership filter.</returns>
    public static RouteHandlerBuilder WithEntityOwnershipFromRouteFilter(this RouteHandlerBuilder builder,
        string idRouteKey)
    {
        return builder.AddEndpointFilterFactory(
                (_, next) => async context =>
                {
                    var filter = new EntityOwnershipFromRouteFilter(idRouteKey);
                    return await filter.InvokeAsync(context, next);
                })
            .Produces(StatusCodes.Status403Forbidden);
    }
    
    /// <summary>
    /// Adds an entity existence filter to the route handler pipeline. This filter checks if the specified
    /// entity exists in the database before proceeding. If the entity does not exist, a 404 Not Found error is returned.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to check for existence.</typeparam>
    /// <typeparam name="TRequest">The type of the request object containing the entity identifier.</typeparam>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to which the filter is added.</param>
    /// <param name="idSelector">A function that selects the entity ID from the request object.</param>
    /// <returns>The modified <see cref="RouteHandlerBuilder"/> with the entity existence filter.</returns>
    public static RouteHandlerBuilder WithEntityExistenceFilter<TEntity, TRequest>(this RouteHandlerBuilder builder,
        Func<TRequest, Guid> idSelector) where TEntity : class, IEntity
    {
        return builder.AddEndpointFilterFactory((_, next) => async context =>
            {
                var databaseSet = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>().Set<TEntity>();
                var filter = new EntityExistenceFilter<TEntity, TRequest>(databaseSet, idSelector);
                return await filter.InvokeAsync(context, next);
            })
            .Produces<List<ErrorResponse>>(StatusCodes.Status404NotFound);
    }
    
    public static RouteHandlerBuilder WithEntityExistenceFromRouteFilter<TEntity>(this RouteHandlerBuilder builder,
        string idRouteKey) where TEntity : class, IEntity
    {
        return builder.AddEndpointFilterFactory((_, next) => async context =>
            {
                var databaseSet = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>().Set<TEntity>();
                var filter = new EntityExistenceFromRoute<TEntity>(databaseSet, idRouteKey);
                return await filter.InvokeAsync(context, next);
            })
            .Produces<List<ErrorResponse>>(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Adds an authentication filter to the route handler pipeline. This filter ensures that the incoming
    /// request is authenticated before proceeding.
    /// </summary>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> to which the authentication filter is added.</param>
    /// <returns>The modified <see cref="RouteHandlerBuilder"/> with the authentication filter.</returns>
    public static RouteHandlerBuilder WithAuthenticationFilter(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<AuthenticationFilter>()
            .Produces<List<ErrorResponse>>(StatusCodes.Status401Unauthorized);
    }
}