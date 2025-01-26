using Toucan.Sdk.Contracts.Query.Page;
using Toucan.Sdk.Store.Extensions;

namespace Toucan.Sdk.Store.QueryOptions;
public delegate IEnumerable<Expression<Func<T, object?>>> ExpressionMap<TSource, T>(TSource? source);


internal class SortParser<TSort, TEntity>(ExpressionMap<TSort, TEntity> sortMap) : ISortParser<TSort, TEntity>
    where TSort : notnull
{
    public IEnumerable<SortMarshaller<TEntity>> Parse(params SortOption<TSort>[] options)
    {
        foreach (SortOption<TSort> option in options)
        {
            foreach (SortMarshaller<TEntity>? item in option.CreateSortCommand(sortMap))
            {
                yield return item;
            }
        }
    }
}
