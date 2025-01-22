namespace Toucan.Sdk.Contracts.Query;

public interface IFilteredSet<TFilter, TCriteria>
    where TFilter : BaseFilterNode<TCriteria>
{
    public TFilter? Filter { get; }
}
