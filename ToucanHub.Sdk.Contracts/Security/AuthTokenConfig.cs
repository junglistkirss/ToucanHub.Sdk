namespace ToucanHub.Sdk.Contracts.Security;

[Obsolete("Please clone outside, this class will be deleted soon", true)]
public class AuthTokenConfig
{
    public static readonly AuthTokenConfig Default = new();
    public string Scheme { get; set; } = "Bearer";
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}
