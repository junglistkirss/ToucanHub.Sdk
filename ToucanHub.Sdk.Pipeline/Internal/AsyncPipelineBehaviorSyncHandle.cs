using System.Runtime.CompilerServices;

namespace ToucanHub.Sdk.Pipeline.Internal;

internal sealed class AsyncPipelineBehaviorSyncHandle<TContext>(MiddlewareHandle<TContext> middleware) : IAsyncPipelineBehavior<TContext>
        where TContext : IPipelineContext
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask InvokeAsync(TContext context, RichNextAsyncDelegate<TContext> next)
    {
        middleware(context, () =>
        {
            Task.WaitAll(next(context).AsTask());
        });
        return ValueTask.CompletedTask;
    }
}
