using System.Collections;

namespace Toucan.Infrastructure.Pipeline;

public class MessagePipeline : IMessagePipeline
{
    private readonly IDictionary<Type, List<IMessageMiddleware>> middlewares;

    public MessagePipeline(
        IEnumerable<IMessageMiddleware> middlewares
    )
    {
        this.middlewares = middlewares
            .GroupBy(h => h.CanHandle)
            .ToDictionary(h => h.Key, h => h.ToList());
    }

    public async Task RunAsync(object message, CancellationToken ct)
    {
        Type eventType = message.GetType();

        if (!this.middlewares.TryGetValue(eventType, out List<IMessageMiddleware>? middlewares))
            return;

        foreach (IMessageMiddleware handler in middlewares)
        {
            object? result = await handler.Handle(message, ct);

            if (result == null)
                break;

            if (result == message)
                continue;

            if (result is IEnumerable enumerable)
            {
                foreach (object? newEvent in enumerable)
                {
                    await RunAsync(newEvent, ct);
                }
                return;
            }
            await RunAsync(result, ct);
        }
    }
}
