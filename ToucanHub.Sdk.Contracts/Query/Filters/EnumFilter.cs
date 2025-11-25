using ToucanHub.Sdk.Contracts.Query.Filters.Abstractions;

namespace ToucanHub.Sdk.Contracts.Query.Filters;

public sealed record class EnumFilter<TEnum> : BaseFilter<EnumFilterMethod, TEnum>, IFilter<EnumFilter<TEnum>>
{
    public static readonly DateFilter Empty = new() { Method = default, Value = default };
}
