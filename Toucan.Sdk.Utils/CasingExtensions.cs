using System.Text;

namespace Toucan.Sdk.Utils;

public static class CasingExtensions
{
    public static string ToPascalCase(this string value) => value.AsSpan().ToPascalCase();

    public static string ToPascalCase(this ReadOnlySpan<char> value)
    {
        if (value.Length == 0)
            return string.Empty;

        StringBuilder stringBuilder = new(value.Length);
        char c = '\0';
        int num = 0;
        foreach (char c2 in value)
        {
            if (c2 is '-' or '_' || char.IsWhiteSpace(c2))
            {
                if (c != 0)
                    _ = stringBuilder.Append(char.ToUpperInvariant(c));

                c = '\0';
                num = 0;
                continue;
            }

            if (num > 1)
                _ = stringBuilder.Append(c2);
            else if (num == 0)
            {
                c = c2;
            }
            else
            {
                _ = stringBuilder.Append(char.ToUpperInvariant(c));
                _ = stringBuilder.Append(c2);
                c = '\0';
            }

            num++;
        }

        if (c is not '\0')
            _ = stringBuilder.Append(char.ToUpperInvariant(c));

        return stringBuilder.ToString();
    }

    public static string ToKebabCase(this string value) => value.AsSpan().ToKebabCase();

    public static string ToKebabCase(this ReadOnlySpan<char> value)
    {
        if (value.Length == 0)
            return string.Empty;

        StringBuilder stringBuilder = new(value.Length);
        int num = 0;
        foreach (char c in value)
        {
            if (c is '-' or '_' || char.IsWhiteSpace(c))
            {
                num = 0;
                continue;
            }

            if (num > 0)
                _ = stringBuilder.Append(char.ToLowerInvariant(c));
            else
            {
                if (stringBuilder.Length > 0)
                    _ = stringBuilder.Append('-');

                _ = stringBuilder.Append(char.ToLowerInvariant(c));
            }

            num++;
        }

        return stringBuilder.ToString();
    }

    public static string ToCamelCase(this string value) => value.AsSpan().ToCamelCase();

    public static string ToCamelCase(this ReadOnlySpan<char> value)
    {
        if (value.Length == 0)
            return string.Empty;

        StringBuilder stringBuilder = new(value.Length);
        char c = '\0';
        int num = 0;
        foreach (char c2 in value)
        {
            if (c2 is '-' or '_' || char.IsWhiteSpace(c2))
            {
                if (c != 0)
                {
                    if (stringBuilder.Length > 0)
                        _ = stringBuilder.Append(char.ToUpperInvariant(c));
                    else
                    {
                        _ = stringBuilder.Append(char.ToLowerInvariant(c));
                    }
                }

                c = '\0';
                num = 0;
                continue;
            }

            if (num > 1)
                _ = stringBuilder.Append(c2);
            else if (num == 0)
            {
                c = c2;
            }
            else
            {
                if (stringBuilder.Length > 0)
                    _ = stringBuilder.Append(char.ToUpperInvariant(c));
                else
                {
                    _ = stringBuilder.Append(char.ToLowerInvariant(c));
                }

                _ = stringBuilder.Append(c2);
                c = '\0';
            }

            num++;
        }

        if (c is not '\0')
        {
            if (stringBuilder.Length > 0)
                _ = stringBuilder.Append(char.ToUpperInvariant(c));
            else
            {
                _ = stringBuilder.Append(char.ToLowerInvariant(c));
            }
        }

        return stringBuilder.ToString();
    }
}
