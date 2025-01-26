using Toucan.Sdk.Utils;

namespace Toucan.Sdk.Application.Mediator.Consumers;

public class FaultConsumer<T>(ILogger<FaultConsumer<T>> logger) : IConsumer<Fault<T>>
{
    public virtual Task Consume(ConsumeContext<Fault<T>> context)
    {
        logger.LogError(string.Join(Environment.NewLine, context.Message.Exceptions.ProjectTo(x => x.Message)));
        return Task.CompletedTask;
    }
}
