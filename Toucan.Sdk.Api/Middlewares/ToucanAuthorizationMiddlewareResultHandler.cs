using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Toucan.Sdk.Api.Extensions;

namespace Toucan.Sdk.Api.Middlewares;

public sealed class ToucanAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler;

    public ToucanAuthorizationMiddlewareResultHandler()
    {
        defaultHandler = new AuthorizationMiddlewareResultHandler();
    }
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Succeeded)
        {
            await next(context);
        }
        else
        {
            string? message = null;
            if (authorizeResult.AuthorizationFailure is not null)
            {
                IEnumerable<AuthorizationFailureReason> reasons = authorizeResult.AuthorizationFailure.FailureReasons;
                message = string.Join(Environment.NewLine, reasons.Select(x => x.Message));
            }
            if (authorizeResult.Forbidden)
                await context.CompleteForbiddenAsync(message ?? "Forbidden");
            else if (authorizeResult.Challenged)
                await context.CompleteUnauthorizedAsync(message ?? "Unauthorized");
            else
                await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}