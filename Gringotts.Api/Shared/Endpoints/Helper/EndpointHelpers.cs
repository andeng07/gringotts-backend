using Gringotts.Api.Shared.Database;
using Gringotts.Api.Shared.Models;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Shared.Endpoints.Helper;

/// <summary>
/// Provides helper methods for common entity operations such as retrieving, updating, and deleting entities.
/// </summary>
public static class EndpointHelpers
{
    /// <summary>
    /// Retrieves a single entity by its GUID.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="guid">The unique identifier of the entity.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="responseMapper">Optional function to map the entity to a response type.</param>
    /// <returns>A result with the entity or an error if not found.</returns>
    public static async Task<IResult> GetEntity<TEntity, TResponse>(
        Guid guid,
        AppDbContext dbContext,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == guid),
            responseMapper: responseMapper
        );
    }

    /// <summary>
    /// Retrieves a single entity based on a query defined by the entityQuery function.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRequest">The request type for the query.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The query parameters.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="entityQuery">A function to define the query logic.</param>
    /// <param name="responseMapper">Optional function to map the entity to a response type.</param>
    /// <returns>A result with the entity or an error if not found.</returns>
    public static async Task<IResult> GetEntityByQuery<TEntity, TRequest, TResponse>(
        TRequest request,
        AppDbContext dbContext,
        Func<TEntity, TRequest, bool> entityQuery,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => entityQuery(x, request)),
            responseMapper: responseMapper
        );
    }

    /// <summary>
    /// Deletes a single entity identified by its GUID.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="guid">The unique identifier of the entity.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="responseMapper">Optional function to map the entity to a response type.</param>
    /// <returns>A result indicating success or failure.</returns>
    public static async Task<IResult> DeleteEntity<TEntity, TResponse>(
        Guid guid,
        AppDbContext dbContext,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == guid),
            entityOperation: entity =>
            {
                dbContext.Set<TEntity>().Remove(entity);
                dbContext.SaveChanges();
            },
            responseMapper: responseMapper
        );
    }

    /// <summary>
    /// Deletes a single entity based on a query defined by the entityQuery function.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRequest">The request type for the query.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The query parameters.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="entityQuery">A function to define the query logic.</param>
    /// <param name="responseMapper">Optional function to map the entity to a response type.</param>
    /// <returns>A result indicating success or failure.</returns>
    public static async Task<IResult> DeleteEntityByQuery<TEntity, TRequest, TResponse>(
        TRequest request,
        AppDbContext dbContext,
        Func<TEntity, TRequest, bool> entityQuery,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => entityQuery(x, request)),
            entityOperation: entity =>
            {
                dbContext.Set<TEntity>().Remove(entity);
                dbContext.SaveChanges();
            },
            responseMapper: responseMapper
        );
    }

    /// <summary>
    /// Updates a single entity based on the entity's ID.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="id">The ID of the entity to update.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="updateEntity">A function to update the entity.</param>
    /// <param name="responseMapper">Optional function to map the entity to a response type.</param>
    /// <returns>A result with the updated entity or an error if not found.</returns>
    public static async Task<IResult> UpdateEntity<TEntity, TResponse>(
        Guid id, 
        AppDbContext dbContext,
        Action<TEntity> updateEntity, 
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id),
            entityOperation: updateEntity,
            responseMapper: responseMapper
        );
    }

    /// <summary>
    /// Updates a single entity based on a query defined by the entityQuery function.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRequest">The request type for the query.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The query parameters.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="entityQuery">A function to define the query logic.</param>
    /// <param name="updateEntity">A function to update the entity.</param>
    /// <param name="responseMapper">Optional function to map the entity to a response type.</param>
    /// <returns>A result with the updated entity or an error if not found.</returns>
    public static async Task<IResult> UpdateEntityByQuery<TEntity, TRequest, TResponse>(
        TRequest request,
        AppDbContext dbContext,
        Func<TEntity, TRequest, bool> entityQuery,
        Action<TEntity> updateEntity,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => entityQuery(x, request)),
            entityOperation: updateEntity,
            responseMapper: responseMapper
        );
    }

    /// <summary>
    /// Helper method to perform an operation on a single entity.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="entityFinder">A function to find the entity.</param>
    /// <param name="entityOperation">Optional function to perform an operation on the entity (e.g., update or delete).</param>
    /// <param name="responseMapper">Optional function to map the entity to a response type.</param>
    /// <returns>A result with the entity or an error if not found.</returns>
    private static async Task<IResult> PerformEntityOperation<TEntity, TResponse>(
        Func<Task<TEntity?>> entityFinder,
        Action<TEntity>? entityOperation = null,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        var entity = await entityFinder();

        if (entity == null)
        {
            var error = new Error(
                $"{typeof(TEntity).Name}.NotFound",
                $"{typeof(TEntity).Name} not found",
                Error.ErrorType.NotFound
            );

            return Microsoft.AspNetCore.Http.Results.NotFound(error);
        }

        entityOperation?.Invoke(entity);

        var response = responseMapper != null ? responseMapper(entity) : (TResponse)(object)entity;

        return Microsoft.AspNetCore.Http.Results.Ok(response);
    }

    /// <summary>
    /// Retrieves multiple entities based on a query defined by the entityQuery function.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRequest">The request type for the query.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The query parameters.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="entityQuery">A function to define the query logic.</param>
    /// <param name="responseMapper">Optional function to map the entities to a response type.</param>
    /// <returns>A result with the entities or an error if none found.</returns>
    public static async Task<IResult> GetEntities<TEntity, TRequest, TResponse>(
        TRequest request,
        AppDbContext dbContext,
        Func<TEntity, TRequest, bool> entityQuery,
        Func<IEnumerable<TEntity>, IEnumerable<TResponse>>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformBulkEntityOperation(
            () => Task.FromResult<IEnumerable<TEntity>>(dbContext.Set<TEntity>().Where(x => entityQuery(x, request))),
            responseMapper: responseMapper
        );
    }

    /// <summary>
    /// Deletes multiple entities based on a query defined by the entityQuery function.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRequest">The request type for the query.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The query parameters.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="entityQuery">A function to define the query logic.</param>
    /// <param name="responseMapper">Optional function to map the entities to a response type.</param>
    /// <returns>A result indicating success or failure.</returns>
    public static async Task<IResult> DeleteEntities<TEntity, TRequest, TResponse>(
        TRequest request,
        AppDbContext dbContext,
        Func<TEntity, TRequest, bool> entityQuery,
        Func<IEnumerable<TEntity>, IEnumerable<TResponse>>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformBulkEntityOperation(
            async () =>
            {
                var entities = dbContext.Set<TEntity>().Where(x => entityQuery(x, request)).ToList();
                dbContext.Set<TEntity>().RemoveRange(entities);
                await dbContext.SaveChangesAsync();
                return entities;
            },
            responseMapper: responseMapper
        );
    }

    /// <summary>
    /// Updates multiple entities based on a query defined by the entityQuery function.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRequest">The request type for the query.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The query parameters.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="entityQuery">A function to define the query logic.</param>
    /// <param name="updateEntity">A function to update the entities.</param>
    /// <param name="responseMapper">Optional function to map the entities to a response type.</param>
    /// <returns>A result with the updated entities or an error if none found.</returns>
    public static async Task<IResult> UpdateEntities<TEntity, TRequest, TResponse>(
        TRequest request,
        AppDbContext dbContext,
        Func<TEntity, TRequest, bool> entityQuery,
        Action<TEntity> updateEntity,
        Func<IEnumerable<TEntity>, IEnumerable<TResponse>>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformBulkEntityOperation(
            async () =>
            {
                var entities = dbContext.Set<TEntity>().Where(x => entityQuery(x, request)).ToList();
                foreach (var entity in entities)
                {
                    updateEntity(entity);
                }

                dbContext.Set<TEntity>().UpdateRange(entities);
                await dbContext.SaveChangesAsync();
                return entities;
            },
            responseMapper: responseMapper
        );
    }

    /// <summary>
    /// Performs an operation on a collection of entities retrieved from a data source.
    /// This method is intended for use with bulk operations such as retrieving, updating, or deleting multiple entities.
    /// It handles the retrieval of entities, applies an optional response mapping, and returns a result indicating success or failure.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being operated on. Must implement the <see cref="IEntity"/> interface.</typeparam>
    /// <typeparam name="TResponse">The type of response to return after mapping the entities.</typeparam>
    /// <param name="entityFinder">A function that retrieves a collection of entities asynchronously. Returns <see cref="Task{IEnumerable{TEntity}}"/>.</param>
    /// <param name="responseMapper">An optional function that maps the collection of entities to the desired response format. If not provided, the entities are returned as is.</param>
    /// <returns>
    /// A result indicating success or failure. If entities are found, it returns an <see cref="IResult"/> with an "Ok" response containing the mapped entities.
    /// If no entities are found, it returns a <see cref="IResult"/> with a "NotFound" error response.
    /// </returns>
    /// <remarks>
    /// This method simplifies the implementation of bulk entity operations by providing a consistent way to handle the retrieval, 
    /// error handling, and response formatting for collections of entities.
    /// </remarks>
    private static async Task<IResult> PerformBulkEntityOperation<TEntity, TResponse>(
        Func<Task<IEnumerable<TEntity>>> entityFinder,
        Action<IEnumerable<TEntity>>? entityOperation = null,
        Func<IEnumerable<TEntity>, IEnumerable<TResponse>>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        var entities = (await entityFinder()).ToList();

        if (entities.Count == 0)
        {
            var error = new Error(
                $"{typeof(TEntity).Name}.NotFound",
                $"No {typeof(TEntity).Name}s matched the query",
                Error.ErrorType.NotFound
            );

            return Microsoft.AspNetCore.Http.Results.NotFound(error);
        }

        entityOperation?.Invoke(entities);

        var response = responseMapper != null ? responseMapper(entities) : entities.Cast<TResponse>();

        return Microsoft.AspNetCore.Http.Results.Ok(response);
    }
}