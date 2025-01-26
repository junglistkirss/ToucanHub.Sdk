using Toucan.Sdk.Contracts.Messages.Envelopes;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Shared.Messages;
public abstract record class CommandEnvelope : EnvelopeMessage<CommandMessage>, ICommand { }

[DataContract, Serializable]
public record class CommandEnvelope<T> : CommandEnvelope, IEnvelope<T>
    where T : CommandMessage
{

    [IgnoreDataMember]
    public new T Message { get => (T)base.Message; init => base.Message = value; }
}
