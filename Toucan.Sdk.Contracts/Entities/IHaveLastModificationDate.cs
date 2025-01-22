namespace Toucan.Sdk.Contracts.Entities;

public interface IHaveLastModificationDate
{
    DateTimeOffset? LastModified { get; }
}
