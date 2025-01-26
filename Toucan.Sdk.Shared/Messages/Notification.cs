using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Shared.Messages;

public abstract record class Notification<TId>(TId Id, NotificationType Type) : IEvent
    where TId : struct;

public enum NotificationType
{
    Created,
    Updated,
    Deleted,
}
