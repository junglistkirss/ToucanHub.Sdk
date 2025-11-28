namespace ToucanHub.Sdk.Pipeline;

public interface IAsyncPipeline<TContext>
    where TContext : IPipelineContext
{
    ValueTask ExecuteAsync(TContext context);
}
