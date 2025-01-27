using Toucan.Sdk.Api.Contracts.Search;
using Toucan.Sdk.Api.Extensions;
using Toucan.Sdk.Contracts.Query.Page;

namespace Toucan.Sdk.Api.Contracts.Tests;


internal record class TestFilter
{
}


internal enum TestSortEnum
{
    Default,
    Name,
    Active,
}

public class ApiHelperTests
{
    [Fact]
    public void PrepareQuery1()
    {
        SimpleQueryApi<TestSortEnum, TestFilter> queryApi = new()
        {
            Filter = null,
            Pagination = new PaginateQueryParamsApi
            {
                Limit = 10,
                Offset = 0,
            },
            Sorting = new SortQueryParamsApi<TestSortEnum>
            {
                SortBy = default!,
                SortDirection = null
            }
        };
        queryApi.PrepareQuery(out Pagination paging, out SortOption<TestSortEnum>[] sorting, out TestFilter? filter);
        Assert.Equal(10, paging.Limit);
        Assert.Equal(0, paging.Offset);
        Assert.Single(sorting);
        Assert.Equal(TestSortEnum.Default, sorting[0].Field);
        Assert.Equal(SortDirection.Asc, sorting[0].Direction);
    }

    [Fact]
    public void PrepareQuery2()
    {
        SimpleQueryApi<TestSortEnum, TestFilter> queryApi = new()
        {
            Filter = null,
            Pagination = new PaginateQueryParamsApi
            {
                Limit = -10,
                Offset = -5,
            },
            Sorting = null,
        };
        queryApi.PrepareQuery(out Pagination paging, out SortOption<TestSortEnum>[] sorting, out TestFilter? filter);
        Assert.Equal(1, paging.Limit);
        Assert.Equal(0, paging.Offset);
        Assert.Single(sorting);
        Assert.Equal(TestSortEnum.Default, sorting[0].Field);
        Assert.Equal(SortDirection.Asc, sorting[0].Direction);
    }

    [Fact]
    public void PrepareQuery3()
    {
        SimpleQueryApi<TestSortEnum, TestFilter> queryApi = new()
        {
            Filter = null,
            Pagination = new PaginateQueryParamsApi
            {
                Limit = 10000000,
                Offset = -5,
            },
            Sorting = null,
        };
        queryApi.PrepareQuery(out Pagination paging, out SortOption<TestSortEnum>[] sorting, out TestFilter? filter);
        Assert.Equal(10000, paging.Limit);
        Assert.Equal(0, paging.Offset);
        Assert.Single(sorting);
        Assert.Equal(TestSortEnum.Default, sorting[0].Field);
        Assert.Equal(SortDirection.Asc, sorting[0].Direction);
    }


    [Fact]
    public void PrepareQuery4()
    {
        SimpleQueryApi<TestSortEnum, TestFilter> queryApi = new()
        {
            Filter = null,
            Pagination = new PaginateQueryParamsApi
            {
                Limit = 1000,
                Offset = 500,
            },
            Sorting = new SortQueryParamsApi<TestSortEnum>
            {
                SortBy = TestSortEnum.Name,
                SortDirection = SortDirection.Desc,
            },
        };
        queryApi.PrepareQuery(out Pagination paging, out SortOption<TestSortEnum>[] sorting, out TestFilter? filter);
        Assert.Equal(1000, paging.Limit);
        Assert.Equal(500, paging.Offset);
        Assert.Single(sorting);
        Assert.Equal(TestSortEnum.Name, sorting[0].Field);
        Assert.Equal(SortDirection.Desc, sorting[0].Direction);
    }

}
