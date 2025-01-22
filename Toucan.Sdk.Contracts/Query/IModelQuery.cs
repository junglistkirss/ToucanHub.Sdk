namespace Toucan.Sdk.Contracts.Query;

public interface IModelQuery<TFilter, TSearch> : IFilteredSet<TFilter, TSearch>
    where TFilter : BaseFilterNode<TSearch>
{
}
