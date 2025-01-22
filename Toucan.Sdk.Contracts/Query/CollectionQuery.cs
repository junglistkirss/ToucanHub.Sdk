using Toucan.Sdk.Contracts.Query.Page;

namespace Toucan.Sdk.Contracts.Query;

public readonly record struct CollectionPage<TSortEnum> : IPaginatedSet<TSortEnum>
    where TSortEnum : struct
{
    public static readonly CollectionPage<TSortEnum> Empty = new();
    public Pagination Pagination { get; init; }
    public SortOption<TSortEnum>[] SortOptions { get; init; }
}

public readonly record struct CollectionQuery<TFilter, TSearch, TSortEnum> : ICollectionQuery<TFilter, TSearch, TSortEnum>
    where TFilter : BaseFilterNode<TSearch>
    where TSortEnum : struct
{
    public static readonly CollectionQuery<TFilter, TSearch, TSortEnum> Empty = new();

    public TFilter? Filter { get; init; }
    public Pagination Pagination { get; init; }
    public SortOption<TSortEnum>[] SortOptions { get; init; }
}
