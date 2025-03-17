namespace Toucan.Sdk.Contracts.Query;

public interface IFilteredSet<TFilter>
{
    public TFilter Filter { get; }
}
