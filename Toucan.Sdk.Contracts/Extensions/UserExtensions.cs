using System.Security.Claims;
using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Security;

namespace Toucan.Sdk.Contracts.Extensions;
public static class UserExtensions
{
    public static AppScope[] GetScopes(this ClaimsPrincipal principal)
        => [.. principal.FindAll(TokenClaimNames.scope).Select(x => x.Value).Where(x => !string.IsNullOrEmpty(x)).Select(v => new AppScope(v))];

    public static Role[] GetRoles(this ClaimsPrincipal principal)
        => [.. principal.FindAll(ClaimTypes.Role).Select(x => x.Value).Where(x => !string.IsNullOrEmpty(x)).Select(v => new Role(v))];

    public static RefToken GetRefToken(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        if (principal.Identity?.IsAuthenticated == true)
        {

            string? subjectName = principal.GetName();
            bool hasName = !string.IsNullOrWhiteSpace(subjectName);

            string? subjectEmail = principal.GetEmail();
            bool hasEmail = !string.IsNullOrWhiteSpace(subjectEmail);

            string? subjectId = principal.GetNameIdentifier();
            bool hasId = !string.IsNullOrWhiteSpace(subjectId);

            if (hasEmail && hasName)
                return RefToken.User($"{subjectName} <{subjectEmail}>");

            else if (hasEmail)
                return RefToken.User($"{subjectEmail}");

            else if (hasName)
                return RefToken.User($"{subjectName}");

            else if (hasId)
                return RefToken.User($"{subjectId}");

            else
                return RefToken.User(null);
        }

        return RefToken.Anonymous;
    }
    public static string? GetName(this ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated ?? false)
            return principal.FindFirst(ClaimTypes.Name)?.Value;

        throw new InvalidOperationException("User is not authenticated");
    }

    public static string? GetNameIdentifier(this ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated ?? false)
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        throw new InvalidOperationException("User is not authenticated");
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated ?? false)
            return principal.FindFirst(ClaimTypes.Email)?.Value;

        throw new InvalidOperationException("User is not authenticated");
    }
}
