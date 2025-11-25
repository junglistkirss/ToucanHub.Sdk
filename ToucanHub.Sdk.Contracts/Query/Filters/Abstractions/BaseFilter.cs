namespace ToucanHub.Sdk.Contracts.Query.Filters.Abstractions;


public abstract record class BaseFilter<TMethod, TValue>
    where TMethod : struct, IConvertible
{
    public TMethod Method { get; init; } = default!;
    public TValue? Value { get; init; } = default!;

    public override string ToString() => $"{Method}({Value})";
}
