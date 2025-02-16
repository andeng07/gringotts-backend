using System.Linq.Expressions;
using Gringotts.Api.Shared.Core;
using LinqKit;

namespace Gringotts.Api.Shared.Utilities;

public class DataFilter<T>
{
    private ExpressionStarter<T> _predicate = PredicateBuilder.New<T>(true); // Default to 'true' (no filtering)

    public DataFilter<T> Where(Expression<Func<T, bool>> predicate)
    {
        _predicate = _predicate.And(predicate);
        return this;
    }

    public Expression<Func<T, bool>> Build() => _predicate;
}

public interface IDataFilter<T> where T : class, IEntity
{
    Expression<Func<T, bool>> ApplyFilters();
}

public static class FilteredQueryExtensions
{
    public static IQueryable<T> ApplyDataFilters<T>(this IQueryable<T> query, Expression<Func<T, bool>> filterExpression)
    {
        return query.AsExpandable().Where(filterExpression);
    }
}