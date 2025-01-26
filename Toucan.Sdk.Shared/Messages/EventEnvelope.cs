using Toucan.Sdk.Contracts.Messages.Envelopes;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Shared.Messages;

public abstract record class EventEnvelope : EnvelopeMessage<EventMessage>, IEvent, IEnvelope<EventMessage> { }


[DataContract, Serializable]
public record class EventEnvelope<T> : EventEnvelope, IEnvelope<T>
    where T : EventMessage
{
    [IgnoreDataMember]
    public new T Message { get => (T)base.Message; init => base.Message = value; }
}