using ToucanHub.Sdk.Contracts.Entities;
using ToucanHub.Sdk.Contracts.Names;

namespace ToucanHub.Sdk.Contracts.Wrapper;


public abstract record class ContractEntity : IObjectEntity<DomainId, ActorReference>
{
    public DomainId Id { get; init; }
    public ActorReference CreatedBy { get; init; }
    public DateTimeOffset Created { get; init; }
    public ActorReference? LastModifiedBy { get; init; }
    public DateTimeOffset? LastModified { get; init; }

    public bool Equals(IEntity<DomainId>? other)
    {
        if (other != null)
            return Id!.Equals(other!.Id);

        return false;
    }
}

public abstract record class ContractEntity<T> : ContractEntity
{
    public T Data { get; init; } = default!;

}
