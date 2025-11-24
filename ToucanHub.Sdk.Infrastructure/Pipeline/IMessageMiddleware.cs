using System;
using Toucan.Infrastructure.Markers;
using Toucan.Infrastructure.Transport;

namespace Toucan.Infrastructure.Pipeline;

public interface IMessageMiddleware
{
    Type CanHandle { get; }

    ValueTask<object?> Handle(object message, CancellationToken ct);
}

public interface IMessageMiddleware<in TMessage> : IMessageMiddleware
{
    Type IMessageMiddleware.CanHandle => typeof(TMessage);

    async ValueTask<object?> IMessageMiddleware.Handle(object message, CancellationToken ct)
    {
        await Handle((TMessage)message, ct);
        return message;
    }

    ValueTask Handle(TMessage message, CancellationToken ct);
}
