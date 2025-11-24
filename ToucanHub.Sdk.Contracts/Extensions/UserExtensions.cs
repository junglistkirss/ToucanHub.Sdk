using System.Security.Claims;
using ToucanHub.Sdk.Contracts.Names;
using ToucanHub.Sdk.Contracts.Security;

namespace ToucanHub.Sdk.Contracts.Extensions;
public static class UserExtensions
{
    public static Area[] GetScopes(this ClaimsPrincipal principal)
        => [.. principal.FindAll(TokenClaimNames.scope).Select(x => x.Value).Where(x => !string.IsNullOrEmpty(x)).Select(v => new Area(v))];

    public static Role[] GetRoles(this ClaimsPrincipal principal)
        => [.. principal.FindAll(ClaimTypes.Role).Select(x => x.Value).Where(x => !string.IsNullOrEmpty(x)).Select(v => new Role(v))];

    public static ActorReference GetActorReference(this ClaimsPrincipal principal)
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
                return ActorReference.User($"{subjectName} <{subjectEmail}>");

            else if (hasEmail)
                return ActorReference.User($"{subjectEmail}");

            else if (hasName)
                return ActorReference.User($"{subjectName}");

            else if (hasId)
                return ActorReference.User($"{subjectId}");

            else
                return ActorReference.User(null);
        }

        return ActorReference.Anonymous;
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
