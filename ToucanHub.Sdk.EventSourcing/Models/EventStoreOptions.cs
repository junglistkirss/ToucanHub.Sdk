namespace ToucanHub.Sdk.EventSourcing.Models;

public class EventStoreOptions<TStreamKey, TEvent, TProjection>
    where TProjection : notnull
    where TEvent : notnull
    where TStreamKey : struct
{
}