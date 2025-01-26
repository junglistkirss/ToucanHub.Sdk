using Toucan.Sdk.Api.Contracts.Search;
using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Query.Filters;
using Toucan.Sdk.Contracts.Query.Filters.Abstractions;
using Toucan.Sdk.Contracts.Query.Page;

namespace Toucan.Sdk.Api.Extensions;

public static class SearchHelper
{
    public static void PrepareQuery<TSort, TFilter>(this SimpleQueryApi<TSort, TFilter>? queryApi, out Pagination pagination, out SortOption<TSort>[] sort, out TFilter? filter, TSort defaultSort = default!, TFilter? defaultFilter = default!)
        where TSort : notnull
        where TFilter : notnull
    {
        TFilter? filterValue = default!;
        TSort? sortValue = defaultSort;
        if (queryApi is not null)
        {
            if (queryApi.Sorting is not null)
                sortValue = queryApi.Sorting.SortBy;

            if (queryApi.Filter is not null)
                filterValue = queryApi.Filter;
        }
        pagination = Pagination.Create(queryApi?.Pagination?.Offset, queryApi?.Pagination?.Limit);
        sort = SortOption<TSort>.Simple(sortValue, queryApi?.Sorting?.SortDirection);
        filter = filterValue;
    }

    public static StringFilter? CreateStringFilter(string? pattern = null, string? filterMethod = null)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return StringFilter.None;
        if (Enum.TryParse(filterMethod, true, out StringFilterMethod result))
            return new()
            {
                Method = result,
                Value = pattern
            };
        return new()
        {
            Method = StringFilterMethod.Equals,
            Value = pattern
        };
    }

    public static StringFilter? CreateStringFilter(this string pattern, StringFilterMethod filterMethod = StringFilterMethod.Equals)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return StringFilter.None;

        return new StringFilter()
        {
            Method = filterMethod,
            Value = pattern.Trim()
        };
    }

    public static StringFilter? CreateStringFilter(this Slug name) => new()
    {
        Method = StringFilterMethod.Equals,
        Value = name.ToString()
    };

    public static StringFilter? ToModel(this SearchStringApiModel apiModel) => CreateStringFilter(apiModel?.Value?.ToString().Trim(), apiModel?.Method?.Trim());
}
