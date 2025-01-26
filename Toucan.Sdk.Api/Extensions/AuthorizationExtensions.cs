using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Toucan.Sdk.Contracts.Security;

namespace Toucan.Sdk.Api.Extensions;

public static class AuthorizationExtensions
{
    public static AuthenticationBuilder AddToucanBearer(this AuthenticationBuilder services, TokenSignKeyProvider secret, AuthTokenConfig auth, JwtTokenConfig jwtTokenConfig)
    {
        return services.AddJwtBearer(auth.Scheme, o =>
        {
            o.RequireHttpsMetadata = true;

            o.SaveToken = true;
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secret()));

            o.TokenValidationParameters = jwtTokenConfig.TokenValidationParameters(auth, key);
            o.UseSecurityTokenValidators = true;
            o.Events = new JwtBearerEvents
            {
                //OnTokenValidated = context =>
                //{
                //    var fingerprint = context.Request.Headers["X-Fingerprint"].ToString();
                //    var tokenFingerprint = context.Principal.FindFirst("fingerprint")?.Value;

                //    if (string.IsNullOrEmpty(fingerprint) ||
                //        tokenFingerprint != Hasher.ToSha256(fingerprint))
                //    {
                //        context.Fail("Invalid token fingerprint");
                //    }

                //    return Task.CompletedTask;
                //},

                //OnTokenValidated = context =>
                //{
                //    JsonWebToken? token = context.SecurityToken as JsonWebToken;
                //    if (token is null)
                //        context.Fail("BadToken");

                //    return Task.CompletedTask;
                //},
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException tokenExpiredException)
                    {
                        context.Response.Headers.Append("X-Token-Expired", tokenExpiredException.Expires.ToString()!);
                        context.Fail(tokenExpiredException);
                    }
                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    //if (context. is SecurityTokenExpiredException tokenExpiredException)
                    //{
                    //    context.Response.Headers.Append("X-Token-Expired", tokenExpiredException.Expires.ToString()!);
                    //}
                    return Task.CompletedTask;
                },
            };
        });
    }

}
