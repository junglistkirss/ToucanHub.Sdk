using System.Diagnostics;

namespace Toucan.Sdk.Api.Middlewares;

public sealed class AddStopWatchHeader(RequestDelegate next)
{

    private readonly RequestDelegate next = next;

    public async Task InvokeAsync(HttpContext context /*, IDomainService appProvider, ILogger<ScopeResolver> logger*/)
    {
        Stopwatch watch = Stopwatch.StartNew();

        //To add Headers AFTER everything you need to do this
        context.Response.OnStarting(state =>
        {
            HttpContext? httpContext = (HttpContext)state;
            httpContext.Response.Headers.Append("X-Response-Time-Milliseconds", new[] { watch.ElapsedMilliseconds.ToString() });

            return Task.CompletedTask;
        }, context);


        await next(context);
    }
}
