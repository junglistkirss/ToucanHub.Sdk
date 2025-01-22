namespace Toucan.Sdk.Contracts.Query;

public interface ICollectionQuery<TFilter, TCriteria, TSort> : IPaginatedSet<TSort>, IFilteredSet<TFilter, TCriteria>
    where TFilter : BaseFilterNode<TCriteria>
    where TSort : notnull
{ }
