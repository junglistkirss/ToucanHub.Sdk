namespace Toucan.Sdk.EventSourcing.Models;

public readonly record struct EventSlice< TEvent>(Versioning Version, ETag ETag, TEvent[] Events)
    where TEvent : notnull;


