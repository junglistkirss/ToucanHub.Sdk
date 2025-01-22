using Toucan.Sdk.Contracts.Query.Filters.Abstractions;

namespace Toucan.Sdk.Contracts.Query.Filters;


public sealed record class NumericFilter<TNumeric> : BaseFilter<NumericFilterMethod, TNumeric?>, IFilter<NumericFilter<TNumeric>>
    where TNumeric : unmanaged, IComparable, IComparable<TNumeric>, IConvertible, IEquatable<TNumeric>, IFormattable
{
    public static readonly NumericFilter<TNumeric> Empty = new() { Method = default, Value = default };
}
