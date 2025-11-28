using System.Runtime.CompilerServices;

namespace ToucanHub.Sdk.Pipeline.Internal;

internal sealed class AsyncPipelineSyncBehavior<TContext>(RichMiddlewareHandle<TContext> middleware) : IAsyncPipelineBehavior<TContext>
        where TContext : IPipelineContext
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask InvokeAsync(TContext context, RichNextAsyncDelegate<TContext> next)
    {
        middleware(context, (ctx) =>
        {
            Task.WaitAll(next(ctx).AsTask());
        });
        return ValueTask.CompletedTask;
    }
}
