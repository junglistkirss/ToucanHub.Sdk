namespace ToucanHub.Sdk.Contracts.Entities;

public interface IObjectEntity<TId, TRef> :
    IEntity<TId>,
    IHaveCreator<TRef>,
    IHaveCreationDate,
    IHaveLastModifier<TRef>,
    IHaveLastModificationDate
    where TId : struct
    where TRef : struct
{
}
