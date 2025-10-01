namespace Toucan.Sdk.Contracts.Security;

public readonly struct Role(string Name, PermissionSet? Permissions = null)
{
    public string Name { get; } = Name;

    public PermissionSet Permissions { get; } = Permissions ?? PermissionSet.Empty;

    public static Role WithPermissions(string name, params string[] permissions) => new(name, new PermissionSet(permissions));
}
