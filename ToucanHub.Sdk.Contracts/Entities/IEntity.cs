namespace ToucanHub.Sdk.Contracts.Entities;

public interface IEntity<TId>
    where TId : struct
{
    TId Id { get; }
}
