using ToucanHub.Sdk.Contracts.Names;
using ToucanHub.Sdk.Contracts.Query.Filters;

namespace ToucanHub.Sdk.Contracts.Criterias;

public interface IEntityCriteria
{
    public DateFilter? Created { get; }
    public ActorReference? Creator { get; }
    public DateFilter? LastModified { get; }
    public ActorReference? Modifier { get; }
}
