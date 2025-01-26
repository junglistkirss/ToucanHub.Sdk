using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Application.Context;
public interface IUserContext
{
    RefToken Issuer { get; }
    Role[] Roles { get; }
    ValueTask<bool> HasScope(AppScope scope);
    ValueTask<bool> HasPermission(Permission permission);
    ValueTask<PermissionSet> GetAllPermissions();
    ValueTask<AppScope[]> GetAllScopes();
}