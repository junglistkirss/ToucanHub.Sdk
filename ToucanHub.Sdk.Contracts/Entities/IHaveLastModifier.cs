namespace ToucanHub.Sdk.Contracts.Entities;
public interface IHaveLastModifier<TRef> : IHaveLastModificationDate
    where TRef : struct
{
    TRef? LastModifiedBy { get; }
}
