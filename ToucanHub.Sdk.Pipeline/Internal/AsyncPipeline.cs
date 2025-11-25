using ToucanHub.Sdk.Pipeline.Exceptions;

namespace ToucanHub.Sdk.Pipeline.Internal;

internal sealed class AsyncPipeline<TContext>(IEnumerable<AsyncRichMiddlewareHandle<TContext>> middlewares) : IAsyncPipeline<TContext>
    where TContext : IPipelineContext
{
    private readonly AsyncRichMiddlewareHandle<TContext>[] _middlewares = [.. middlewares];

    public ValueTask ExecuteAsync(TContext context)
    {
        PipelineExecution execution = new(_middlewares);
        return execution.RunAsync(context);
    }

    private sealed class PipelineExecution(AsyncRichMiddlewareHandle<TContext>[] middlewares)
    {
        private int _index = -1;

        public async ValueTask RunAsync(TContext context)
        {
            try
            {
                await NextAsync(context);
            }
            catch (FlowException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FlowException("Error occurs during pipeline execution, see inner exception for details", ex);
            }
        }

        private ValueTask NextAsync(TContext context)
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
                return NextAsync(context);
            }
            return middleware(context, Next);
        }
    }
}


