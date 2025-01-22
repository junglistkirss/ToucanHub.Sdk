namespace Toucan.Sdk.Contracts.Query;

public readonly record struct ModelQuery<TFilter, TSearch> : IModelQuery<TFilter, TSearch>
    where TFilter : BaseFilterNode<TSearch>
{
    public TFilter? Filter { get; init; }
}
