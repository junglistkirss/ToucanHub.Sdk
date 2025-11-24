namespace ToucanHub.Sdk.Contracts.Entities;
public interface IHaveCreator<TRef> : IHaveCreationDate
    where TRef : struct
{
    TRef CreatedBy { get; }
}
