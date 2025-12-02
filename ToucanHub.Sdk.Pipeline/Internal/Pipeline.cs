using System.Runtime.CompilerServices;
using ToucanHub.Sdk.Pipeline.Exceptions;

namespace ToucanHub.Sdk.Pipeline.Internal;

internal sealed class Pipeline<TContext>(IEnumerable<IPipelineBehavior<TContext>> middlewares) : IPipeline<TContext>
        where TContext : IPipelineContext
{

    public void Execute(TContext context)
    {
        using IEnumerator<IPipelineBehavior<TContext>> _middlewareEnumerator = middlewares.GetEnumerator();
        bool nextCalled = false;

        void Next(TContext ctx)
        {
            if (nextCalled)
                throw new FlowException("Next delegate should only be called once");

            nextCalled = true;

            if (_middlewareEnumerator.MoveNext())
            {

                nextCalled = false;
                _middlewareEnumerator.Current.Invoke(ctx, (ctx) =>
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

internal sealed class PipelineBehavior<TContext>(RichMiddlewareHandle<TContext> middleware) : IPipelineBehavior<TContext>
        where TContext : IPipelineContext
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(TContext context, RichNextDelegate<TContext> next)
    {
        middleware(context, next);
    }
}

internal sealed class PipelineBehaviorHandle<TContext>(MiddlewareHandle<TContext> middleware) : IPipelineBehavior<TContext>
        where TContext : IPipelineContext
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(TContext context, RichNextDelegate<TContext> next)
    {
        middleware(context, () => next(context));
    }
}


internal sealed class PipelineBehaviorTermination<TContext>(MiddlewareAction<TContext> middleware) : IPipelineBehavior<TContext>
        where TContext : IPipelineContext
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(TContext context, RichNextDelegate<TContext> next)
    {
        middleware(context);
    }
}

internal sealed class PipelineBehaviorContinuation<TContext>(MiddlewareAction<TContext> middleware) : IPipelineBehavior<TContext>
        where TContext : IPipelineContext
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(TContext context, RichNextDelegate<TContext> next)
    {
        middleware(context);
        next(context);
    }
}
