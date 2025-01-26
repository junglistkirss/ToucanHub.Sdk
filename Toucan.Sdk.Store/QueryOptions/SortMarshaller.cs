using Toucan.Sdk.Contracts.Query.Page;

namespace Toucan.Sdk.Store.QueryOptions;

public class SortMarshaller<T>(SortDirection direction, Expression<Func<T, object?>> field)
{
    public SortDirection Direction { get; } = direction;
    public Expression<Func<T, object?>> Field { get; } = field;
}
