namespace Toucan.Infrastructure.Pipeline;

public interface IMessagePipeline
{
    Task RunAsync(object message, CancellationToken ct);
}
