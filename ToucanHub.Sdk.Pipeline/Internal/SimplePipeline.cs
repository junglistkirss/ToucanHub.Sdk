using ToucanHub.Sdk.Pipeline.Exceptions;

namespace ToucanHub.Sdk.Pipeline.Internal;

internal sealed class SimplePipeline<TContext>(IEnumerable<RichMiddlewareHandle<TContext>> middlewares) : IPipeline<TContext>
        where TContext : IPipelineContext
{
    private readonly RichMiddlewareHandle<TContext>[] _middlewares = [.. middlewares];

    public void Execute(TContext context)
    {
        int index = -1;
        bool nextCalled = false;

        void Next(TContext ctx)
        {
            int currentIndex = ++index;

            if (nextCalled)
                throw new FlowException("Next delegate should only be called once");

            nextCalled = true;

            if (currentIndex < _middlewares.Length)
            {
                nextCalled = false;
                _middlewares[currentIndex](ctx, (ctx) =>
                {
                    Next(ctx);
                });
            }
        }

        try
        {
            Next(context);
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
}