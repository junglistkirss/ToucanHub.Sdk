namespace ToucanHub.Sdk.EventSourcing.Models;

[Serializable]
public sealed class EventStoreException : Exception
{
    public EventStoreException()
    { }
    public EventStoreException(string? message) : base(message)
    {
    }
    public EventStoreException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
