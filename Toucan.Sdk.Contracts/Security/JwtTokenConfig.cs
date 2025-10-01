namespace Toucan.Sdk.Contracts.Security;

public class JwtTokenConfig
{
    public static readonly JwtTokenConfig Default = new();


    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = false;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public bool RequireExpirationTime { get; set; } = false;

    /// <summary>
    /// Default 30 days
    /// </summary>
    public int IdTokenExpiration { get; set; } = 2592000; // 30 days

    /// <summary>
    /// Default 1 day
    /// </summary>
    public int AccessTokenExpiration { get; set; } = 86400; // 1 day

    /// <summary>
    /// Default 7 days
    /// </summary>
    public int RefreshTokenExpiration { get; set; } = 604800; // 7 days
}
