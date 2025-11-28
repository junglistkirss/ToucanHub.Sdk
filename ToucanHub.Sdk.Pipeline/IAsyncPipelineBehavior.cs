namespace ToucanHub.Sdk.Pipeline;

public interface IAsyncPipelineBehavior<TContext>
    where TContext : IPipelineContext
{
    ValueTask InvokeAsync(TContext context, RichNextAsyncDelegate<TContext> next);
}
