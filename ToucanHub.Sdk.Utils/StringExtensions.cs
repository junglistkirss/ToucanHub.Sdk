using System.Diagnostics.CodeAnalysis;

namespace ToucanHub.Sdk.Utils;

public static partial class StringExtensions
{
    public static bool IsNotNullTrimmed(this string? value, [NotNullWhen(true)] out string? output) => !value.IsNullTrimmed(out output);

    public static bool IsNullTrimmed(this string? value, [NotNullWhen(false)] out string? output)
    {
        output = value.TrimOrNull();
        return output is null;
    }

    public static string? TrimOrNull(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        string? str = value.Trim();
        if (string.IsNullOrWhiteSpace(str))
            return null;
        return str;
    }

    public static string Or(this string? value, string fallback) => value.TrimOrNull() ?? fallback;
}
