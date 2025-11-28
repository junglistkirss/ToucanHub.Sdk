namespace ToucanHub.Sdk.Pipeline;

public interface IPipeline<TContext>
    where TContext : IPipelineContext
{
    void Execute(TContext context);
}
