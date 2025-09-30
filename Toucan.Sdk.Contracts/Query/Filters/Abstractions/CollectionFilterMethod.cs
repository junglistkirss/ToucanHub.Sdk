namespace Toucan.Sdk.Contracts.Query.Filters.Abstractions;

public enum CollectionFilterMethod : byte
{
    Any = 0,
    All = 1 << 1,
}