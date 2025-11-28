using System.Runtime.CompilerServices;

namespace ToucanHub.Sdk.Pipeline.Internal;

internal sealed class AsyncPipelineBehavior<TContext>(AsyncRichMiddlewareHandle<TContext> middleware) : IAsyncPipelineBehavior<TContext>
        where TContext : IPipelineContext
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask InvokeAsync(TContext context, RichNextAsyncDelegate<TContext> next)
    {
        return middleware(context, next);
    }
}
