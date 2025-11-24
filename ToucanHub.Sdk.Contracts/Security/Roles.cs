namespace ToucanHub.Sdk.Contracts.Security;

[Obsolete("Avoid this, may lead to security concerns",true)]
public static class Roles
{
    public const string Developer = nameof(Developer);

    public const string SuperAdmin = "*";

    public static readonly IReadOnlyDictionary<string, Role> Defaults = new Dictionary<string, Role>
    {
        [Developer] = Role.WithPermissions(Developer, SuperAdmin)
    };
}
