using Toucan.Sdk.Pipeline.Exceptions;

namespace Toucan.Sdk.Pipeline;

internal sealed class AsyncPipeline<TContext>(IEnumerable<AsyncRichMiddlewareHandle<TContext>> middlewares) : IAsyncPipeline<TContext>
    where TContext : IPipelineContext
{
    private readonly AsyncRichMiddlewareHandle<TContext>[] _middlewares = [.. middlewares];

    public ValueTask ExecuteAsync(TContext context)
    {
        var state = new PipelineExecution(_middlewares);
        return state.RunAsync(context);
    }

    private sealed class PipelineExecution(AsyncRichMiddlewareHandle<TContext>[] middlewares)
    {
        private int _index = -1;

        public ValueTask RunAsync(TContext context)
        {
            return InvokeNextAsync(context);
        }

        private ValueTask InvokeNextAsync(TContext context)
        {
            int currentIndex = Interlocked.Increment(ref _index);

            if (currentIndex >= middlewares.Length)
                return ValueTask.CompletedTask;

            AsyncRichMiddlewareHandle<TContext> middleware = middlewares[currentIndex];

            bool nextCalled = false;

            ValueTask Next(TContext context)
            {
                if (nextCalled)
                    throw new FlowException("Next delegate should only be called once");

                nextCalled = true;
                return InvokeNextAsync(context);
            }
            try
            {
                return middleware.Invoke(context, Next);
            }
            catch (Exception ex)
            {
                throw new FlowException("Error occurs during pipeline execution, see inner exception for details", ex);
            }
        }
    }
}


