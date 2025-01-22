namespace Toucan.Sdk.Pipeline.Internal;

internal sealed class SimplePipeline<TContext>(IEnumerable<RichMiddlewareHandle<TContext>> middlewareTypes) : IPipeline<TContext>
        where TContext : IPipelineContext
{
    private static void Terminate(TContext _) { }

    public void Execute(TContext context)
    {
        RichNextDelegate<TContext> next = Terminate;
        foreach (RichMiddlewareHandle<TContext> instance in middlewareTypes.Reverse())
            next = instance.WithNext(next);
        next(context);
    }
}