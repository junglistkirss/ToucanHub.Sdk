namespace ToucanHub.Sdk.Contracts.Security;

public delegate string TokenSignKeyProvider();

public static class Grants
{
    public const string ScopeClaimType = "toucan:scope";
    public const string GrantPermissionClaimType = "toucan:grant";
}
