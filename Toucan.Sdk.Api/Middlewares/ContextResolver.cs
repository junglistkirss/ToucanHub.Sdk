using Microsoft.Extensions.Logging;
using Toucan.Sdk.Api.Features;
using Toucan.Sdk.Application.Context;

namespace Toucan.Sdk.Api.Middlewares;

internal sealed class ContextResolver(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<ContextResolver> logger)
    {
        try
        {
            IUserContext user = context.Features.Get<IUserContext>() ?? NoopUserContext.Empty;
            context.Features.Set(ApiContext.Create(Tenant.Unspecified, user, context.RequestAborted));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error resolving context");
        }
        finally
        {
            await next(context);
        }

    }
}
