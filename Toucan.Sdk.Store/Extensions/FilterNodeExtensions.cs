using Toucan.Sdk.Contracts.Query;
using Toucan.Sdk.Contracts.Query.Filters;
using Toucan.Sdk.Store.QueryOptions;

namespace Toucan.Sdk.Store.Extensions;


public static class FilterNodeExtensions
{
    public static Expression<Func<T, bool>>? Use<T, TFilter, TCriteria>(this TFilter node, FilterHandler<T, TCriteria>? parser)
        where TFilter : BaseFilterNode<TCriteria>
        where T : class
    {
        if (node == null)
            return null;
        if (parser == null)
            return null;

        Expression<Func<T, bool>>? expression = null;
        if (node is IFilterNode<TCriteria> single && single.Filter != null)
            expression = parser(single.Filter);

        if (node is IFilterGroup<TCriteria, TFilter> group && group.Nodes != null)
        {
            return group.Aggregator switch
            {
                FilterAggregator.OR => expression.OrElseNext(group.Nodes.Select(f => f.Use(parser)).ToArray()),
                _ => expression.AndAlsoNext(group.Nodes.Select(f => f.Use(parser)).ToArray()),
            };
        }

        return expression;
    }
}

