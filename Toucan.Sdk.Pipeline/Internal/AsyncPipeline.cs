namespace Toucan.Sdk.Pipeline;

internal sealed class AsyncPipeline<TContext>(IEnumerable<AsyncRichMiddlewareHandle<TContext>> middlewares) : IAsyncPipeline<TContext>
        where TContext : IPipelineContext
{
    private static ValueTask Terminate(TContext _) => ValueTask.CompletedTask;

    public async ValueTask ExecuteAsync(TContext context)
    {
        RichNextAsyncDelegate<TContext> next = Terminate;
        foreach (AsyncRichMiddlewareHandle<TContext> instance in middlewares.Reverse())
            next = instance.WithNextAsync(next);
        await next(context);
    }
}


