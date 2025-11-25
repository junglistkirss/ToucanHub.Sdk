using ToucanHub.Sdk.Contracts.Query.Filters;

namespace ToucanHub.Sdk.Contracts.Criterias;

public interface IVersionCriteria
{
    public NumericFilter<long>? Version { get; }
}
