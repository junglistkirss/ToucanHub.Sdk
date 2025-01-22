namespace Toucan.Sdk.Contracts.Query.Filters.Abstractions;

public enum EnumFilterMethod : byte
{
    Equals,
    HasFlag,
    GreaterThan,
    LessThan,
    GreaterThanOrEquals,
    LessThanOrEquals,
    NotEquals,
    NotHasFlag,
}
