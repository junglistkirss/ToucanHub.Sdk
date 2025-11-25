using System.Runtime.Serialization;

namespace ToucanHub.Sdk.Contracts.Messages.Envelopes;

[DataContract, Serializable]
public record class EnvelopeMessage : IEnvelope<IMessage>
{
    [DataMember]
    public MessageHeaders Headers { get; init; } = default!;

    [DataMember]
    public IMessage Message { get; init; } = default!;
}


[DataContract, Serializable]
public record class EnvelopeMessage<T> : EnvelopeMessage, IEnvelope<T>
    where T : IMessage
{
    [IgnoreDataMember]
    public new T Message { get => (T)base.Message; init => base.Message = value; }
}
