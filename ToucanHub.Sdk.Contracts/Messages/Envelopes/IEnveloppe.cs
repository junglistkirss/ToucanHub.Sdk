namespace ToucanHub.Sdk.Contracts.Messages.Envelopes;

public interface IEnvelope<T> : IEnvelope
    where T : IMessage
{
    T Message { get; }
}
public interface IEnvelope
{
    MessageHeaders Headers { get; }
}
