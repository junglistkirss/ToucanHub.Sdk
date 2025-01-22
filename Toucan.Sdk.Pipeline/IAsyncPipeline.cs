namespace Toucan.Sdk.Pipeline;

public interface IPipeline<TContext>
    where TContext : IPipelineContext
{
    void Execute(TContext context);
}

public interface IAsyncPipeline<TContext>
    where TContext : IPipelineContext
{
    ValueTask ExecuteAsync(TContext context);
}



