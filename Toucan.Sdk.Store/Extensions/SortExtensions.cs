using Toucan.Sdk.Contracts.Query.Page;
using Toucan.Sdk.Store.QueryOptions;

namespace Toucan.Sdk.Store.Extensions;
public static class SortExtensions
{
    public static IEnumerable<SortMarshaller<T>> CreateSortCommands<TSort, T>(this IEnumerable<SortOption<TSort>> options, ExpressionMap<TSort, T> sortMap)
         where TSort : notnull => options.SelectMany(option => option.CreateSortCommand(sortMap)).ToArray();

    public static IEnumerable<SortMarshaller<T>> CreateSortCommand<TSort, T>(this SortOption<TSort> option, ExpressionMap<TSort, T> sortMap)
        where TSort : notnull
    {
        IEnumerable<Expression<Func<T, object?>>> map = sortMap(option.Field);
        if (map != null)
        {
            foreach (Expression<Func<T, object?>> field in map)
            {
                if (field != null)
                    yield return new SortMarshaller<T>(option.Direction, field);
            }
        }
    }
}
