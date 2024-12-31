using Gringotts.Api.Shared.Models;
using Gringotts.Api.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Gringotts.Api.Shared.Filters;

/// <summary>
/// An endpoint filter that checks if an entity exists in the database based on the ID extracted from the request.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to check for existence in the database.</typeparam>
/// <typeparam name="TRequest">The type of the request object containing the entity ID.</typeparam>
/// <param name="databaseSet">The <see cref="DbSet{TEntity}"/> representing the collection of entities in the database.</param>
/// <param name="idSelector">A function that selects the entity ID from the request object.</param>
public class EntityExistenceFilter<TEntity, TRequest>(DbSet<TEntity> databaseSet, Func<TRequest, Guid> idSelector)
    : IEndpointFilter where TEntity : class, IEntity
{
    
    /// <summary>
    /// Executes the filter logic to check if the entity exists in the database.
    /// If the entity does not exist, returns a 404 Not Found response.
    /// If the entity exists, the request is passed to the next filter in the pipeline.
    /// </summary>
    /// <param name="context">The context for invoking the endpoint filter.</param>
    /// <param name="next">The delegate to invoke the next filter in the pipeline.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation of the filter.</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().Single();
        
        var requestId = idSelector(request);

        if (!await databaseSet.AnyAsync(x => x.Id == requestId))
        {
            var error = new List<Error>
            {
                new($"{typeof(TEntity).Name}.NotFound", 
                    $"{typeof(TEntity).Name} with id: {requestId} not found.",
                    Error.ErrorType.NotFound)
            };
            return Microsoft.AspNetCore.Http.Results.NotFound(error);
        }

        return await next(context);
    }
}