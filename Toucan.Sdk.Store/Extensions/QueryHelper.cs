using Toucan.Sdk.Contracts.Query.Filters;
using Toucan.Sdk.Contracts.Query.Page;

namespace Toucan.Sdk.Contracts.Query;

public static class QueryHelper
{
    public static CollectionQuery<TFilter, TSearch, TSortEnum> Collection<TFilter, TSearch, TSortEnum>()
        where TFilter : BaseFilterNode<TSearch>
        where TSortEnum : struct 
        => new();

    public static CollectionPage<TSortEnum> Page<TSortEnum>(this Pagination pagination)
        where TSortEnum : struct
    {
        return new()
        { Pagination = pagination };
    }

    public static CollectionQuery<TFilter, TSearch, TSortEnum> Collection<TFilter, TSearch, TSortEnum>(this Pagination pagination)
        where TFilter : BaseFilterNode<TSearch>
        where TSortEnum : struct
    {
        return new()
        { Pagination = pagination };
    }

    public static CollectionQuery<TFilter, TSearch, TSortEnum> Paginate<TFilter, TSearch, TSortEnum>(this CollectionQuery<TFilter, TSearch, TSortEnum> collection, Pagination pagination)
        where TFilter : BaseFilterNode<TSearch>
       where TSortEnum : struct 
        => collection with { Pagination = pagination };


    public static CollectionQuery<TFilter, TSearch, TSortEnum> Sort<TFilter, TSearch, TSortEnum>(this CollectionQuery<TFilter, TSearch, TSortEnum> collection, params SortOption<TSortEnum>[] sortOptions)
        where TFilter : BaseFilterNode<TSearch>
        where TSortEnum : struct 
        => collection with { SortOptions = sortOptions };

    //public static TQuery Where<TQuery, TFilter>(this TQuery collection, TFilter filter)
    //    where TQuery : IFilteredSet<TFilter>
    //{
    //    collection.Filter = new FilterNode<TFilter>(filter);
    //    return collection;
    //}


    public static ModelQuery<TFilter, TSearch> Model<TFilter, TSearch>(this TFilter filter)
        where TFilter : BaseFilterNode<TSearch>
    {
        return new()
        {
            Filter =  filter
        };
    }
}
