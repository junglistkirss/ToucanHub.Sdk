using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Toucan.Sdk.Contracts.Security;

public delegate string TokenSignKeyProvider();

public class AuthTokenConfig
{
    public static readonly AuthTokenConfig Default = new();
    public string Scheme { get; set; } = "Bearer";
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}

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

public static class TokenClaimNames
{
    public const string Random = "rnd";
    public const string Subject = "sub";
    ///<summary>
    /// The list of scopes separated by spaces
    ///</summary>
    public const string permission = "permission";
    public const string scope = "scope";
    public const string fingerprint = "fingerprint";

    public const string FamilyName = "family_name";
    public const string Nickname = "nickname";
    public const string PreferredUsername = "preferred_username";
    public const string Profile = "profile";
    public const string Picture = "picture";
    public const string Website = "website";
    public const string Email = "email";
    public const string EmailVerified = "email_verified";
    public const string Zoneinfo = "zoneinfo";
    public const string Locale = "locale";
    public const string PhoneNumber = "phone_number";
    public const string PhoneNumberVerified = "phone_number_verified";
    public const string Address = "address";
    public const string UpdatedAt = "updated_at";
}
public static class JwtToken
{
    public static TokenValidationParameters TokenValidationParameters(this JwtTokenConfig config, AuthTokenConfig auth, SecurityKey encryptionKey)
    {
        return new TokenValidationParameters
        {
            ValidAudience = auth.Audience,
            ValidIssuer = auth.Issuer,
            ValidIssuers = [auth.Issuer],
            //SignatureValidator = delegate (string token, TokenValidationParameters parameters)
            //{
            //    var jwt = new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token);
            //    return jwt;
            //},
            AuthenticationType = auth.Scheme,
            TokenDecryptionKey = encryptionKey,
            IssuerSigningKey = encryptionKey,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256, SecurityAlgorithms.HmacSha384, SecurityAlgorithms.HmacSha512, SecurityAlgorithms.Aes256CbcHmacSha512],
            IgnoreTrailingSlashWhenValidatingAudience = true,
            RequireAudience = true,
            RequireSignedTokens = true,
            SaveSigninToken = true,
            ValidateLifetime = config.ValidateLifetime,
            ValidateIssuer = config.ValidateIssuer,
            ValidateAudience = config.ValidateAudience,

            RequireExpirationTime = config.RequireExpirationTime,
            ValidateIssuerSigningKey = config.ValidateIssuerSigningKey,
        };
    }
}

public static class Grants
{
    public const string ScopeClaimType = "toucan:scope";
    public const string GrantPermissionClaimType = "toucan:grant";
}
