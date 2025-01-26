using Toucan.Sdk.Contracts.Messages;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Application.Services;
public interface IPublisher
{
    public ValueTask PublishEvent<T>(T message)
          where T : class, IEvent;
    public ValueTask PublishEventMessage<T>(T message)
          where T : EventMessage;
}

