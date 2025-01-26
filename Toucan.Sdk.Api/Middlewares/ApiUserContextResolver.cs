using System.Security.Claims;
using Toucan.Identity.Api.Features;
using Toucan.Sdk.Contracts.Security;

namespace Toucan.Sdk.Api.Middlewares;


public delegate ValueTask<AppScope[]> PrincipaScopesResolver(ClaimsPrincipal? principal);

public delegate ValueTask<PermissionSet> PrincipaPermissionsResolver(ClaimsPrincipal? principal);

internal sealed class ApiUserContextResolver(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, PrincipaPermissionsResolver persmissionsResolver, PrincipaScopesResolver scopesResolver)
    {
        ClaimsPrincipal? user = httpContext.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            httpContext.Features.Set(ApiUserContext.CreateFromPrincipal(httpContext.User, scopesResolver, persmissionsResolver));
        }
        else
        {
            httpContext.Features.Set(ApiUserContext.CreateEmpty);
        }

        await next(httpContext);
    }
}
