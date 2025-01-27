namespace Toucan.Sdk.Contracts.Query.Filters;

public interface IFilterGroup<T, TCollection>
    where TCollection : BaseFilterNode<T>
{
    public FilterAggregator Aggregator { get; }
    public TCollection[] Nodes { get; }
}

