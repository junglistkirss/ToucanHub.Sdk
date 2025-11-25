namespace ToucanHub.Sdk.Contracts.Entities;

public interface IHaveLastModificationDate
{
    DateTimeOffset? LastModified { get; }
}
