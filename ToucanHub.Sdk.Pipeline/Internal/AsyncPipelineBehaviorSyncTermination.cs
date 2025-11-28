using System.Runtime.CompilerServices;

namespace ToucanHub.Sdk.Pipeline.Internal;

internal sealed class AsyncPipelineBehaviorSyncTermination<TContext>(MiddlewareAction<TContext> middleware) : IAsyncPipelineBehavior<TContext>
        where TContext : IPipelineContext
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask InvokeAsync(TContext context, RichNextAsyncDelegate<TContext> next)
    {
        middleware(context);
        return ValueTask.CompletedTask;
    }
}
