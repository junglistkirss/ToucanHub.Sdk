using Toucan.Sdk.Contracts.Query.Filters.Abstractions;

namespace Toucan.Sdk.Contracts.Query.Filters;

public record class CollectionFilter<TFilter> : BaseFilter<CollectionFilterMethod, TFilter[]>, IFilter<CollectionFilter<TFilter>>
    where TFilter : class, IFilter<TFilter>
{
}
