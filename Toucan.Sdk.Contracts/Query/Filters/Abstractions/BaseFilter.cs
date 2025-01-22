using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.Query.Filters.Abstractions;

public interface IFilter<TSelf> where TSelf : class, IFilter<TSelf>
{
    TOut CreatePredicate<TOut, TIn>(Projector<(TSelf Filter, TIn ArgValue), TOut> projector, TIn args) => projector(((TSelf)this, args));
}
public sealed record class SameFilter<TValue>(TValue? Value = default) : IFilter<SameFilter<TValue>>
{
    public override string ToString() => $"{Value}";
}

public abstract record class BaseFilter<TMethod, TValue>
    where TMethod : struct, IConvertible
{
    public TMethod Method { get; init; } = default!;
    public  TValue? Value { get; init; } = default!;

    public override string ToString() => $"{Method}({Value})";
}
