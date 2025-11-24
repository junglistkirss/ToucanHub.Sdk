namespace ToucanHub.Sdk.EventSourcing.Models;

public interface IStoredProjection<THeaders, TData>
{
    Guid Id { get; }
    ProjectionMetadata Metadata { get; }
    TData ProjectionData { get; }
    Versioning Offset { get; }
}