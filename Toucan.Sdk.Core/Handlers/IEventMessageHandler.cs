using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Shared.Messages;

namespace Toucan.Sdk.Core.Handlers;

public interface IEventMessageHandler<TMessage> : IEventHandler<EventEnvelope<TMessage>>
    where TMessage : EventMessage
{ }


//public interface ISnapshotMessageHandler<TMessage> : ISnapshotHandler<SnapshotEnvelope<TMessage>>
//    where TMessage : IMessage
//{
//}
