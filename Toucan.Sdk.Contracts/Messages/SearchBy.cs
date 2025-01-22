using Toucan.Sdk.Contracts.Query;
using Toucan.Sdk.Contracts.Query.Page;
using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.Messages;

public abstract record class SearchBy<TFilter, TCriteria, TSort, TOut> : QueryMessage<Results<TOut>>, ICollectionQuery<TFilter, TCriteria, TSort>
    where TFilter : BaseFilterNode<TCriteria>
    where TSort : notnull
{
    public required Pagination Pagination { get; init; }

    public SortOption<TSort>[] SortOptions { get; init; } = [];

    public TFilter? Filter { get; init; } = null;
}
