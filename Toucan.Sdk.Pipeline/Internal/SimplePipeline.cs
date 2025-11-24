using Toucan.Sdk.Pipeline.Exceptions;

namespace Toucan.Sdk.Pipeline.Internal;

internal sealed class SimplePipeline<TContext>(IEnumerable<RichMiddlewareHandle<TContext>> middlewares) : IPipeline<TContext>
        where TContext : IPipelineContext
{
    private readonly RichMiddlewareHandle<TContext>[] _middlewares = [.. middlewares];

    public void Execute(TContext context)
    {
        int index = -1;

        void Next(TContext ctx)
        {
            bool nextCalled = false;
            int currentIndex = ++index;

            if (nextCalled)
                throw new FlowException("Next delegate should only be called once");

            nextCalled = true;

            if (currentIndex < _middlewares.Length)
                try
                {
                    _middlewares[currentIndex](ctx, Next);
                }
                catch (Exception ex)
                {
                    throw new FlowException("Error occurs during pipeline execution, see inner exception for details", ex);
                }
        }
        Next(context);
    }
}