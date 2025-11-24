namespace ToucanHub.Sdk.Pipeline.Tests;

public static class PassMiddlewareWithOutput
{
    public static void DefaultAction(CounterContextWithOutput ctx) => ctx.Counter.Add("DefaultPass");


    public static MiddlewareHandle<CounterContextWithOutput> Handle(Action<CounterContextWithOutput>? action = null)
        => new MiddlewareHandle<CounterContextWithOutput>((CounterContextWithOutput context, NextDelegate next) =>
        {
            action?.Invoke(context);
            next();
        });

    public static AsyncMiddlewareHandle<CounterContextWithOutput> HandleAsync(Action<CounterContextWithOutput>? action = null)
        => new AsyncMiddlewareHandle<CounterContextWithOutput>((CounterContextWithOutput context, NextAsyncDelegate next) =>
        {
            action?.Invoke(context);
            return next();
        });

    public static RichMiddlewareHandle<CounterContextWithOutput> RichHandle(Action<CounterContextWithOutput>? action = null)
        => new RichMiddlewareHandle<CounterContextWithOutput>((CounterContextWithOutput context, RichNextDelegate<CounterContextWithOutput> next) =>
        {
            action?.Invoke(context);
            next(context);
        });

    public static AsyncRichMiddlewareHandle<CounterContextWithOutput> RichHandleAsync(Action<CounterContextWithOutput>? action = null)
        => new AsyncRichMiddlewareHandle<CounterContextWithOutput>((CounterContextWithOutput context, RichNextAsyncDelegate<CounterContextWithOutput> next) =>
        {
            action?.Invoke(context);
            return next(context);
        });
}