using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Query.Filters;

namespace Toucan.Sdk.Contracts.Criterias;

public interface IEntityCriteria
{
    public DateFilter? Created { get; }
    public RefToken? Creator { get; }
    public DateFilter? LastModified { get; }
    public RefToken? Modifier { get; }
}
