using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Security;

public record struct ObjectPrivilege(NamedId Name, bool IsAllowed, bool IsAny, ObjectPrivilege[] Children);
public record struct ObjectPrivilegeSummary(bool HasAll, ObjectPrivilege[] Privileges);

public readonly struct Role(string Name, PermissionSet? Permissions = null)
{
    public string Name { get; } = Name;

    public PermissionSet Permissions { get; } = Permissions ?? PermissionSet.Empty;

    public static Role WithPermissions(string name, params string[] permissions) => new(name, new PermissionSet(permissions));
}
