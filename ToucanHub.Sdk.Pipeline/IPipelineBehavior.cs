namespace ToucanHub.Sdk.Pipeline;

public interface IPipelineBehavior<TContext>
    where TContext : IPipelineContext
{
    void Invoke(TContext context, RichNextDelegate<TContext> next);
}