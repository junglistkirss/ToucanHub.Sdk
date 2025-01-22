namespace Toucan.Sdk.Contracts.Entities;

public interface IHaveCreationDate
{
    DateTimeOffset Created { get; }
}
