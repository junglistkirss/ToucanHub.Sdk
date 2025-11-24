namespace Toucan.Sdk.Contracts.Query.Filters;

public sealed record class ExistsFilter<T> : CollectionFilter<SameFilter<T>>
{
}
