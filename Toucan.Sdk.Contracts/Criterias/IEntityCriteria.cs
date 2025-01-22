using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Query.Filters;

namespace Toucan.Sdk.Contracts.Criterias;

public interface IVersionCriteria
{
    public NumericFilter<long>? Version { get; }
}

public interface IEntityCriteria
{
    public StringifyFilter<DomainId>? Id { get; }
    public DateFilter? Created { get; }
    public StringifyFilter<RefToken>? Creator { get; }
    public DateFilter? LastModified { get; }
    public StringifyFilter<RefToken>? Modifier { get; }
}
