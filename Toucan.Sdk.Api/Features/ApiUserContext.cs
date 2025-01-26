using System.Security.Claims;
using Toucan.Sdk.Api.Middlewares;
using Toucan.Sdk.Application.Context;
using Toucan.Sdk.Contracts.Extensions;
using Toucan.Sdk.Contracts.Security;

namespace Toucan.Identity.Api.Features;

internal sealed class ApiUserContext : IUserContext
{
    public static IUserContext CreateEmpty() => new ApiUserContext(RefToken.Anonymous, [], () => ValueTask.FromResult(Array.Empty<AppScope>()), () => ValueTask.FromResult(PermissionSet.Empty));
    public static IUserContext CreateFromPrincipal(ClaimsPrincipal? principal, PrincipaScopesResolver scopesResolver, PrincipaPermissionsResolver permissionsResolver)
    {
        RefToken issuer = principal?.GetRefToken() ?? RefToken.Anonymous;
        Role[] roles = principal?.GetRoles() ?? [];
        return new ApiUserContext(issuer, roles, () => scopesResolver(principal), () => permissionsResolver(principal));
    }

    public Guid Id { get; } = Guid.NewGuid();

    private ApiUserContext(RefToken issuer, Role[] roles, Func<ValueTask<AppScope[]>> scopes, Func<ValueTask<PermissionSet>> permissions)
    {
        Issuer = issuer;
        Roles = roles;
        _permissions = new Lazy<Task<PermissionSet>>(() => Task.Factory.StartNew(() => permissions().AsTask()).Unwrap());
        _scopes = new Lazy<Task<AppScope[]>>(() => Task.Factory.StartNew(() => scopes().AsTask()).Unwrap());
    }

    private readonly Lazy<Task<PermissionSet>> _permissions;
    private readonly Lazy<Task<AppScope[]>> _scopes;

    public async ValueTask<bool> HasScope(AppScope scope)
    {
        AppScope[] set = await _scopes.Value;
        return set.Contains(scope);
    }
    public async ValueTask<bool> HasPermission(Permission permission)
    {
        PermissionSet set = await _permissions.Value;
        return set.Allows(permission);
    }

    public async ValueTask<PermissionSet> GetAllPermissions() => await _permissions.Value;

    public async ValueTask<AppScope[]> GetAllScopes() => await _scopes.Value;

    public RefToken Issuer { get; }
    public Role[] Roles { get; }
}
