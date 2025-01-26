using Toucan.Sdk.Contracts.Query.Page;

namespace Toucan.Sdk.Store.QueryOptions;

public interface ISortParser<TSort, TEntity>
    where TSort : notnull
{
    IEnumerable<SortMarshaller<TEntity>> Parse(params SortOption<TSort>[] options);
}
