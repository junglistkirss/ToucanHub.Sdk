using System.Text.RegularExpressions;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Security;

public static partial class Permissions
{
    [GeneratedRegex("[^A-Za-z0-9_\\^|*]+", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex PartSanitizeRegex();

    [GeneratedRegex("\\{([A-Za-z0-9_]+)\\}", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex WildcardRegex(); // IMPORTANT like SlugRegex with brackets !!

    private static string? Sanitize(this string? part)
    {
        if (string.IsNullOrWhiteSpace(part))
            return null;
        return PartSanitizeRegex().Replace(part, x => string.Empty);
    }

    public static Permission For(this string id, params (string key, string value)[] placeholders)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException($"'{nameof(id)}' ne peut pas avoir une valeur null ou être un espace blanc.", nameof(id));
        }
        return id.For(placeholders.ToDictionary(x => x.key, x => x.value));
    }

    public static Permission For(this string id, IReadOnlyDictionary<string, string> placeholders)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException($"'{nameof(id)}' ne peut pas avoir une valeur null ou être un espace blanc.", nameof(id));
        }

        return new(WildcardRegex().Replace(id, match =>
         {
             if (placeholders.TryGetValue(match.Groups[1].Value, out string? value))
                 return value?.Sanitize() ?? Permission.Any;
             return Permission.Any;
         }).Trim());
    }
}
