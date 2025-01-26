using Toucan.Sdk.Contracts.Messages.Envelopes;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Shared.Messages;
public abstract record class QueryEnvelope : EnvelopeMessage<QueryMessage>, IQuery { }

[DataContract, Serializable]
public record class QueryEnvelope<T> : QueryEnvelope, IEnvelope<T>, IQuery
    where T : QueryMessage
{

    [IgnoreDataMember]
    public new T Message { get => (T)base.Message; init => base.Message = value; }
}