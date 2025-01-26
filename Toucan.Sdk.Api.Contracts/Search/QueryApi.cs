using Toucan.Sdk.Contracts.Query.Page;

namespace Toucan.Sdk.Api.Contracts.Search;

public record class PaginateQueryParamsApi
{
    public static readonly PaginateQueryParamsApi Empty = new PaginateQueryParamsApi();
    public int Offset { get; init; } = 0;
    public int Limit { get; init; } = 100;
}

public record class SortQueryParamsApi<TSort>
    where TSort : notnull
{
    public static readonly SortQueryParamsApi<TSort> Empty = new SortQueryParamsApi<TSort>();
    public TSort? SortBy { get; init; } 
    public SortDirection? SortDirection { get; init; }
}

public record class SimpleQueryApi<TSort, TFilter> 
    where TSort : notnull
{
    public PaginateQueryParamsApi? Pagination { get; init; } = PaginateQueryParamsApi.Empty;
    public SortQueryParamsApi<TSort>? Sorting { get; init; } = SortQueryParamsApi<TSort>.Empty;
    public TFilter? Filter { get; set; }
}