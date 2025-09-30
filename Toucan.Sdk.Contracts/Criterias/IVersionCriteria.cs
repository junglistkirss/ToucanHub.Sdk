using Toucan.Sdk.Contracts.Query.Filters;

namespace Toucan.Sdk.Contracts.Criterias;

public interface IVersionCriteria
{
    public NumericFilter<long>? Version { get; }
}
