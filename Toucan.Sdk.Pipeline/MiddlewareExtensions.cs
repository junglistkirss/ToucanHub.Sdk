using Toucan.Sdk.Pipeline.Exceptions;

namespace Toucan.Sdk.Pipeline;


public static class MiddlewareExtensions
{
    public static RichNextDelegate<T> ThenNext<T>(
       this MiddlewareProvide<RichMiddlewareHandle<T>> middlewareProvider,
       RichNextDelegate<T> next)
       where T : IPipelineContext
    {
        return (ctx) =>
        {
            try
            {
                RichMiddlewareHandle<T> middleware = middlewareProvider();
                middleware(ctx, next);
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message, ex);
            }
        };
    }

    public static RichNextAsyncDelegate<T> ThenNextAsync<T>(
        this MiddlewareProvide<AsyncRichMiddlewareHandle<T>> middlewareProvider,
        RichNextAsyncDelegate<T> next)
        where T : IPipelineContext
    {
        return async (ctx) =>
        {
            try
            {
                AsyncRichMiddlewareHandle<T> middleware = middlewareProvider();
                await middleware(ctx, next);
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message, ex);
            }
        };
    }

    public static RichNextDelegate<T> WithNext<T>(
       this RichMiddlewareHandle<T> middleware,
       RichNextDelegate<T> next)
       where T : IPipelineContext
    {
        return (ctx) =>
        {
            try
            {
                middleware(ctx, next);
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message, ex);
            }
        };
    }

    public static RichNextAsyncDelegate<T> WithNextAsync<T>(
        this AsyncRichMiddlewareHandle<T> middleware,
        RichNextAsyncDelegate<T> next)
        where T : IPipelineContext
    {
        return async (ctx) =>
        {
            try
            {
                await middleware(ctx, next);
            }
            catch (Exception ex)
            {
                throw new FlowException(ex.Message, ex);
            }
        };
    }
}
