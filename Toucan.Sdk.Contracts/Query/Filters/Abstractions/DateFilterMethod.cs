namespace Toucan.Sdk.Contracts.Query.Filters.Abstractions;

public enum DateFilterMethod : byte
{
    Equals,
    GreaterThan,
    LessThan,
    GreaterThanOrEquals,
    LessThanOrEquals,
    NotEquals,
    IsNull,
}

