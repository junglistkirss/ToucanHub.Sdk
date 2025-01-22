using Toucan.Sdk.Contracts.Entities;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Wrapper;


public abstract record class ContractEntity : IObjectEntity<DomainId, RefToken>
{
    public DomainId Id { get; init; }
    public RefToken CreatedBy { get; init; }
    public DateTimeOffset Created { get; init; }
    public RefToken? LastModifiedBy { get; init; }
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
