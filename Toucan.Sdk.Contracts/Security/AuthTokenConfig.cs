namespace Toucan.Sdk.Contracts.Security;

public class AuthTokenConfig
{
    public static readonly AuthTokenConfig Default = new();
    public string Scheme { get; set; } = "Bearer";
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}
