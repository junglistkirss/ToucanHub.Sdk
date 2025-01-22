namespace Toucan.Sdk.Contracts.Security;

public static class Roles
{
    public const string Developer = "Developer";

    public const string SuperAdmin = "*";

    public static readonly IReadOnlyDictionary<string, Role> Defaults = new Dictionary<string, Role>
    {
        [Developer] = Role.WithPermissions(Developer, SuperAdmin)
    };
}
