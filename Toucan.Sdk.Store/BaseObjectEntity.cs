using Toucan.Sdk.Contracts.Entities;

namespace Toucan.Sdk.Store;

public abstract class BaseObjectEntity<TId, TRef> : BaseEntity<TId>, IObjectEntity<TId, TRef>
    where TId : struct
    where TRef : struct
{
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    public TRef CreatedBy { get; set; } = default!;

    public DateTimeOffset? LastModified { get; set; } = null;
    public TRef? LastModifiedBy { get; set; } = null;
}

