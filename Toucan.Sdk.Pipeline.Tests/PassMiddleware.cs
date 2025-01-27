namespace Toucan.Sdk.Pipeline.Tests;

public static class PassMiddleware
{
    public static void DefaultAction(CounterContext ctx) => ctx.Counter.Add("DefaultPass");

    public static MiddlewareHandle<CounterContext> Handle(Action<CounterContext>? action = null)
        => new MiddlewareHandle<CounterContext>((CounterContext context, NextDelegate next) =>
        {
            action?.Invoke(context);
            next();
        });

    public static RichMiddlewareHandle<CounterContext> RichHandle(Action<CounterContext>? action = null)
        => new RichMiddlewareHandle<CounterContext>((CounterContext context, RichNextDelegate<CounterContext> next) =>
        {
            action?.Invoke(context);
            next(context);
        });

    public static AsyncMiddlewareHandle<CounterContext> HandleAsync(Action<CounterContext>? action = null)
        => new AsyncMiddlewareHandle<CounterContext>((CounterContext context, NextAsyncDelegate next) =>
        {
            action?.Invoke(context);
            return next();
        });

    public static AsyncRichMiddlewareHandle<CounterContext> RichHandleAsync(Action<CounterContext>? action = null)
        => new AsyncRichMiddlewareHandle<CounterContext>((CounterContext context, RichNextAsyncDelegate<CounterContext> next) =>
        {
            action?.Invoke(context);
            return next(context);
        });

}