using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Toucan.Sdk.Utils;

public static partial class StringExtensions
{
    public static bool IsValidRegex(this string value)
    {
        try
        {
            _ = new Regex(value);

            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
    public static bool IsEmail(this string? value) => value != null && EmailRegex().IsMatch(value);

    public static bool IsNullOrWhiteSpace(this string? value) => value.IsNullTrimmed(out _);

    public static bool IsNotNullOrWhiteSpace(this string? value) => !value.IsNullOrWhiteSpace();

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

    //public static bool IsPropertyName(this string? value) => value != null && PropertyNameRegex().IsMatch(value);

    public static string Or(this string? value, string fallback) => value.TrimOrNull() ?? fallback;

    //public static string BuildFullUrl(this string baseUrl, string path, bool trailingSlash = false)
    //{
    //    Guard.NotNull(path);

    //    string? url = $"{baseUrl.TrimEnd('/')}/{path.Trim('/')}";

    //    if (trailingSlash &&
    //        url.IndexOf("#", StringComparison.OrdinalIgnoreCase) < 0 &&
    //        url.IndexOf("?", StringComparison.OrdinalIgnoreCase) < 0 &&
    //        url.IndexOf(";", StringComparison.OrdinalIgnoreCase) < 0)
    //    {
    //        url += "/";
    //    }

    //    return url;
    //}

    //public static string JoinNonEmpty(string separator, params string?[] parts)
    //{
    //    ArgumentNullException.ThrowIfNull(separator);

    //    if (parts == null || parts.Length == 0)
    //        return string.Empty;

    //    return string.Join(separator, parts.Where(x => !string.IsNullOrWhiteSpace(x)));
    //}

    [GeneratedRegex("^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+(\\.([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|((\\x22)((((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF]))))*(((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))@((([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-||_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.)+(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+|(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+([a-z]+|\\d|-|\\.{0,1}|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])?([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled, "fr-FR")]
    private static partial Regex EmailRegex();

    //[GeneratedRegex("^[a-zA-Z0-9]+(\\-[a-zA-Z0-9]+)*$", RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    //private static partial Regex PropertyNameRegex();

    //public static string ToIso8601(this DateTime value)
    //{
    //    return value.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture);
    //}
}
