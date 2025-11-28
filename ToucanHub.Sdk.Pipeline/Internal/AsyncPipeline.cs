using System.ComponentModel.DataAnnotations;
using ToucanHub.Sdk.Pipeline.Exceptions;

namespace ToucanHub.Sdk.Pipeline.Internal;

internal sealed class AsyncPipeline<TContext>(IEnumerable<IAsyncPipelineBehavior<TContext>> middlewares) : IAsyncPipeline<TContext>
    where TContext : IPipelineContext
{
    private readonly IEnumerator<IAsyncPipelineBehavior<TContext>> _middlewares = middlewares.GetEnumerator();

    public ValueTask ExecuteAsync(TContext context)
    {
        PipelineExecution execution = new(_middlewares);
        return execution.RunAsync(context);
    }

    private sealed class PipelineExecution(IEnumerator<IAsyncPipelineBehavior<TContext>> middlewareEnumerator)
    {
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
            if(!middlewareEnumerator.MoveNext())
                return ValueTask.CompletedTask;
            IAsyncPipelineBehavior<TContext> middleware = middlewareEnumerator.Current;

            bool nextCalled = false;

            ValueTask Next(TContext context)
            {
                if (nextCalled)
                    throw new FlowException("Next delegate should only be called once");

                nextCalled = true;
                return NextAsync(context);
            }
            return middleware.InvokeAsync(context, Next);
        }
    }
}
