using Toucan.Sdk.Api.Contracts;

namespace Toucan.Sdk.Api.Extensions;

public static class HttpContextExtensions
{
    public static IEnumerable<string> ScopedRouteValuesKeys(this HttpContext ctx)
    {
        return ctx.Request.RouteValues.Keys;
    }

    public static async Task CompleteInternalServerErrorAsync(this HttpContext context, params string[] messages)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        if (!context.Response.HasStarted)
            await context.Response.WriteAsJsonAsync(ApiHelper.Message(ApiStatus.InternalError, messages), typeof(ApiResponseMessage), context.RequestAborted);
        await context.Response.CompleteAsync();
    }

    public static async Task CompleteNotFoundAsync(this HttpContext context, params string[] messages)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        if (!context.Response.HasStarted)
            await context.Response.WriteAsJsonAsync(ApiHelper.Message(ApiStatus.NotFound, messages), typeof(ApiResponseMessage), context.RequestAborted);
        await context.Response.CompleteAsync();
    }

    public static async Task CompleteUnauthorizedAsync(this HttpContext context, params string[] messages)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        if (!context.Response.HasStarted)
            await context.Response.WriteAsJsonAsync(ApiHelper.Message(ApiStatus.Unauthorized, messages), typeof(ApiResponseMessage), context.RequestAborted);
        await context.Response.CompleteAsync();
    }

    public static async Task CompleteForbiddenAsync(this HttpContext context, params string[] messages)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        if (!context.Response.HasStarted)
            await context.Response.WriteAsJsonAsync(ApiHelper.Message(ApiStatus.Forbidden, messages), typeof(ApiResponseMessage), context.RequestAborted);
        await context.Response.CompleteAsync();
    }

    public static async Task CompleteNotAcceptableAsync(this HttpContext context, params string[] messages)
    {
        context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
        if (!context.Response.HasStarted)
            await context.Response.WriteAsJsonAsync(ApiHelper.Message(ApiStatus.Failure, messages), typeof(ApiResponseMessage), context.RequestAborted);
        await context.Response.CompleteAsync();
    }
}
