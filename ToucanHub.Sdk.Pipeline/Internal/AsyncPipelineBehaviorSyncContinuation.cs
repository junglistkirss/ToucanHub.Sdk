using System.Runtime.CompilerServices;

namespace ToucanHub.Sdk.Pipeline.Internal;

internal sealed class AsyncPipelineBehaviorSyncContinuation<TContext>(MiddlewareAction<TContext> middleware) : IAsyncPipelineBehavior<TContext>
        where TContext : IPipelineContext
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async ValueTask InvokeAsync(TContext context, RichNextAsyncDelegate<TContext> next)
    {
        middleware(context);
        await next(context);
    }
}