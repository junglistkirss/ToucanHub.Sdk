using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Toucan.Sdk.Application.Context;
using Toucan.Sdk.Contracts.Security;

namespace Toucan.Sdk.Api.Extensions;
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

public static class GrantExtensions
{
    public static AuthorizationPolicyBuilder RequirePermissions(this AuthorizationPolicyBuilder builder, params Permission[] grants)
        => builder.AddRequirements(new GrantPermissionsRequirement(grants));

    public static AuthorizationPolicyBuilder RequireScopes(this AuthorizationPolicyBuilder builder, params AppScope[] scopes)
        => builder.AddRequirements(new ScopeRequirement(scopes));

}
internal class GrantPermissionsRequirement(params Permission[] grants) : IAuthorizationRequirement
{
    public Permission[] Grants { get; } = grants;
}

internal class ScopeRequirement(params AppScope[] scopes) : IAuthorizationRequirement
{
    public AppScope[] Scopes { get; } = scopes;
}

internal sealed class ScopeHandler(IUserContext ctx) : AuthorizationHandler<ScopeRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext hContext, ScopeRequirement requirement)
    {
        if (!hContext.HasFailed)
        {
            if (requirement.Scopes == null || requirement.Scopes.Length == 0)
                hContext.Succeed(requirement);
            else
            {
                bool allowed = true;
                IEnumerator<AppScope> e = requirement.Scopes.AsEnumerable().GetEnumerator();
                while (e.MoveNext())
                {
                    bool granted = await ctx.HasScope(e.Current);
                    if (!granted)
                    {
                        hContext.Fail(new AuthorizationFailureReason(this, $"User does not granted for scope {e.Current}"));
                        allowed = false;
                    }
                }
                if (allowed)
                    hContext.Succeed(requirement);
            }
        }
    }
}



internal sealed class GrantPermissionsHandler(IUserContext ctx) : AuthorizationHandler<GrantPermissionsRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext hContext, GrantPermissionsRequirement requirement)
    {
        if (!hContext.HasFailed)
        {
            if (requirement.Grants == null || requirement.Grants.Length == 0)
                hContext.Succeed(requirement);
            else
            {
                bool allowed = true;
                IEnumerator<Permission> e = requirement.Grants.AsEnumerable().GetEnumerator();
                while (e.MoveNext())
                {
                    bool granted = await ctx.HasPermission(e.Current);
                    if (!granted)
                    {
                        hContext.Fail(new AuthorizationFailureReason(this, $"User does not granted for permission {e.Current}"));
                        allowed = false;
                    }
                }
                if (allowed)
                    hContext.Succeed(requirement);
            }
        }
    }
}
