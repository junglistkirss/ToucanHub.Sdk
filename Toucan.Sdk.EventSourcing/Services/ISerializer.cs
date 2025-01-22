using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services;

public interface ISerializer<TStreamKey, TEvent, TProjection, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage>
    where TStreamKey : struct
    where TProjection : notnull
    where TEvent : notnull
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>
    where TStoredProjection : IStoredProjection<THeadersStorage, TProjectionDataStorage>
{
    TEvent Deserialize(TStoredEvent eventData);
    TStoredEvent Serialize(TStreamKey streamKey, Versioning version, TEvent obj);

    TProjection Deserialize(TStoredProjection projectionData);
    TStoredProjection Serialize(TStreamKey streamKey, Versioning version, TProjection obj);
}