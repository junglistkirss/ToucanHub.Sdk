using Toucan.Sdk.Contracts.Query.Filters.Abstractions;

namespace Toucan.Sdk.Contracts.Query.Filters;

public sealed record class SameFilter<TValue>(TValue Value) : IFilter<SameFilter<TValue>>
{
    public override string ToString() => $"{Value}";
}