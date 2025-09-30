using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.Query.Filters.Abstractions;

public interface IFilter<TSelf> where TSelf : class, IFilter<TSelf>
{
    TOut CreatePredicate<TOut, TIn>(Projector<(TSelf Filter, TIn ArgValue), TOut> projector, TIn args) => projector(((TSelf)this, args));
}
