namespace ToucanHub.Sdk.Contracts.Query.Filters.Abstractions;

public enum StringFilterMethod : byte
{
    Equals = 0,
    NotEquals = 1 << 1,
    Contains = 1 << 2,
    StartsWith = 1 << 3,
    EndsWith = 1 << 4,
    IsNull = 1 << 5,
    IsEmpty = 1 << 6,
}
