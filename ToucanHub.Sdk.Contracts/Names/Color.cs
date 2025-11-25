using System.Diagnostics;

namespace ToucanHub.Sdk.Contracts.Names;

[DebuggerDisplay("{HexCode,nq}")]
public readonly struct Color : IComparable, IComparable<Color>, IEquatable<Color>, IParsable<Color>
{
    public static readonly Color Empty = string.Empty;

    public static Color Create(string value) => new(value);

    private Color(string value)
    {
        HexCode = ValidColor(value);
    }

    public string HexCode { get; }

    public static string ValidColor(string input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.Trim();

        if (NamingConvention.ColorRegex().IsMatch(input))
            return input;
        throw new InvalidDataException($"{nameof(input)} is not a valid slug : {input}");
    }

    public static bool IsValidColor(string input) => IsValidColor(input, out string? _);
    public static bool IsValidColor(string input, out string? slug)
    {
        slug = null;
        try
        {
            slug = ValidColor(input);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    //public static bool AreSame(Color? s1, Color? s2) => s1.HasValue && s2.HasValue && s1.Value == s2.Value;

    public static bool operator ==(Color slug, string other) => slug.HexCode.Equals(other, StringComparison.OrdinalIgnoreCase);
    public static bool operator !=(Color slug, string other) => !slug.HexCode.Equals(other, StringComparison.OrdinalIgnoreCase);

    //public static implicit operator DomainId(Color input) => DomainId.Create(input);

    public static implicit operator string(Color input) => input.HexCode;

    public static implicit operator Color?(string? input) => TryParse(input, out Color valid) ? valid : default(Color?);

    public static implicit operator Color(string input) => Parse(input);

    public override string ToString() => HexCode;

    public static bool TryParse(string? input, [NotNullWhen(true)] out Color validColor)
    {
        validColor = Empty;
        if (input != null && IsValidColor(input, out string? slug) && slug != null)
        {
            validColor = Create(slug);
            return true;
        }
        return false;
    }

    public static Color Parse(string value)
    {
        if (TryParse(value, out Color result))
            return result;

        throw new ArgumentException($"Not a color : {value}", nameof(value));
    }

    public int CompareTo(Color other) => string.Compare(HexCode, other.HexCode, StringComparison.OrdinalIgnoreCase);
    public bool Equals(Color other) => string.Equals(HexCode, other.HexCode, StringComparison.OrdinalIgnoreCase);
    public static bool operator ==(Color lhs, Color rhs) => lhs.Equals(rhs);

    public static bool operator !=(Color lhs, Color rhs) => !lhs.Equals(rhs);
    public override bool Equals(object? obj)
    {
        if (obj is Color slug)
            return Equals(slug);
        if (obj is string str)
            return string.Equals(HexCode, str, StringComparison.OrdinalIgnoreCase);
        return false;
    }

    public override int GetHashCode() => HexCode.GetHashCode(StringComparison.OrdinalIgnoreCase) + 19;
    public int CompareTo(object? obj) => CompareTo(Create(obj?.ToString() ?? string.Empty));

    public static Color Parse(string s, IFormatProvider? _)
    {
        return Create(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? _, [MaybeNullWhen(false)] out Color result)
    {
        return TryParse(s, out result);
    }
}
