using Toucan.Sdk.Contracts.Query.Filters.Abstractions;

namespace Toucan.Sdk.Contracts.Query.Filters;


public sealed record class StringifyFilter<TValue> : BaseFilter<StringFilterMethod, TValue?>, IFilter<StringifyFilter<TValue>>
{
    public static readonly StringifyFilter<TValue> Empty = new() { Method = default, Value = default };

}
