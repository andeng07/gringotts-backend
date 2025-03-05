using System.Linq.Expressions;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Pagination;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Gringotts.Api.Shared.Utilities;

/// <summary>
/// Provides helper methods for common entity operations such as retrieving, updating, and deleting entities.
/// </summary>
public static class EndpointHelpers
{
    public static async Task<IResult> CreateEntity<TEntity, TResponse>(
        TEntity entity,
        AppDbContext dbContext,
        Func<TEntity, string> uri,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        var dbSet = dbContext.Set<TEntity>();
        await dbSet.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        var response = responseMapper != null ? responseMapper(entity) : (object)entity;
        return Microsoft.AspNetCore.Http.Results.Created(uri(entity), response);
    }

    public static async Task<IResult> GetEntity<TEntity, TResponse>(
        Guid guid,
        AppDbContext dbContext,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == guid),
            responseMapper: responseMapper
        );
    }

    public static async Task<IResult> GetEntityByQuery<TEntity, TResponse>(
        AppDbContext dbContext,
        Expression<Func<TEntity, bool>> entityQuery,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(entityQuery),
            responseMapper: responseMapper
        );
    }

    public static async Task<IResult> DeleteEntity<TEntity, TResponse>(
        Guid guid,
        AppDbContext dbContext,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        var delete = await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().FindAsync(guid),
            entityOperation: entity => dbContext.Set<TEntity>().Remove(entity),
            responseMapper: responseMapper
        );

        await dbContext.SaveChangesAsync();
        
        return delete;
    }

    public static async Task<IResult> DeleteEntityByQuery<TEntity, TResponse>(
        AppDbContext dbContext,
        Expression<Func<TEntity, bool>> entityQuery,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        var delete = await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().FirstOrDefaultAsync(entityQuery),
            entityOperation: entity => dbContext.Set<TEntity>().Remove(entity),
            responseMapper: responseMapper
        );
        
        await dbContext.SaveChangesAsync();
        
        return delete;
    }

    public static async Task<IResult> UpdateEntity<TEntity, TResponse>(
        Guid guid,
        AppDbContext dbContext,
        Action<TEntity> updateEntity,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        var update = await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().FindAsync(guid),
            entityOperation: updateEntity,
            responseMapper: responseMapper
        );
        
        await dbContext.SaveChangesAsync();

        return update;
    }

    public static async Task<IResult> UpdateEntityByQuery<TEntity, TResponse>(
        AppDbContext dbContext,
        Expression<Func<TEntity, bool>> entityQuery,
        Action<TEntity> updateEntity,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        var update = await PerformEntityOperation(
            async () => await dbContext.Set<TEntity>().FirstOrDefaultAsync(entityQuery),
            entityOperation: updateEntity,
            responseMapper: responseMapper
        );

        await dbContext.SaveChangesAsync();

        return update;
    }

    public static async Task<IResult> GetEntities<TEntity, TResponse>(
        AppDbContext dbContext,
        int page,
        int pageSize,
        Expression<Func<TEntity, bool>>? entityQuery = null,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity where TResponse : class
    {
        var query = dbContext.Set<TEntity>().AsNoTracking();

        // Apply the entity-specific query filter if provided by the caller
        if (entityQuery != null)
        {
            query = query.Where(entityQuery);
        }
        
        var queryString = query.ToQueryString();

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination (skip, take) and response mapping
        var entities = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = entities.Select(entity =>
            responseMapper != null ? responseMapper(entity) : (TResponse)(object)entity
        );

        return Microsoft.AspNetCore.Http.Results.Ok(new PaginatedResult<TResponse>(page, pageSize, totalCount, result));
    }

    public static async Task<IResult> DeleteEntities<TEntity, TResponse>(
        AppDbContext dbContext,
        Expression<Func<TEntity, bool>> entityQuery,
        Func<IEnumerable<TEntity>, IEnumerable<TResponse>>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        var delete = await PerformBulkEntityOperation(
            async () =>
            {
                var entities = dbContext.Set<TEntity>().Where(entityQuery);
                dbContext.Set<TEntity>().RemoveRange(entities);
                await dbContext.SaveChangesAsync();
                return entities;
            },
            responseMapper: responseMapper
        );
        
        return delete;
    }

    public static async Task<IResult> UpdateEntities<TEntity, TResponse>(
        AppDbContext dbContext,
        Expression<Func<TEntity, bool>> entityQuery,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateFunction,
        Func<IEnumerable<TEntity>, IEnumerable<TResponse>>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        return await PerformBulkEntityOperation(
            async () =>
            {
                var entities = dbContext.Set<TEntity>().Where(entityQuery);

                await entities.ExecuteUpdateAsync(updateFunction);

                return entities;
            },
            responseMapper: responseMapper
        );
    }

    private static async Task<IResult> PerformEntityOperation<TEntity, TResponse>(
        Func<Task<TEntity?>> entityFinder,
        Action<TEntity>? entityOperation = null,
        Func<TEntity, TResponse>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        var entity = await entityFinder();
        if (entity == null)
        {
            var error = new ErrorResponse(
                $"{typeof(TEntity).Name}.NotFound",
                $"{typeof(TEntity).Name} not found",
                ErrorResponse.ErrorType.NotFound
            );
            return Microsoft.AspNetCore.Http.Results.NotFound(error);
        }

        entityOperation?.Invoke(entity);
        var response = responseMapper != null ? responseMapper(entity) : (TResponse)(object)entity;
        return Microsoft.AspNetCore.Http.Results.Ok(response);
    }

    private static async Task<IResult> PerformBulkEntityOperation<TEntity, TResponse>(
        Func<Task<IQueryable<TEntity>>> entityFinder,
        Action<IQueryable<TEntity>>? entityOperation = null,
        Func<IQueryable<TEntity>, IEnumerable<TResponse>>? responseMapper = null
    ) where TEntity : class, IEntity
    {
        var entities = await entityFinder();
        entityOperation?.Invoke(entities);

        var response = responseMapper != null ? responseMapper(entities) : entities.Cast<TResponse>();
        return Microsoft.AspNetCore.Http.Results.Ok(response);
    }
}