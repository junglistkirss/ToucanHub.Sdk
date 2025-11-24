using System.Diagnostics;

namespace ToucanHub.Sdk.Contracts.Names;

[DebuggerDisplay("{Value,nq}")]
public readonly struct Slug : IComparable, IComparable<Slug>, IEquatable<Slug>, IParsable<Slug>
{
    public static readonly Slug Empty = string.Empty;

    public static Slug Create(string value) => new(value);

    private Slug(string value)
    {
        Value = ValidSlug(value);
    }

    public string Value { get; } = string.Empty;

    public static string ValidSlug(string input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.Trim();

        if (NamingConvention.SlugRegex().IsMatch(input))
            return input;
        throw new InvalidDataException($"{nameof(input)} is not a valid slug : {input}");
    }

    public static bool IsValidSlug(string input) => IsValidSlug(input, out string? _);
    public static bool IsValidSlug(string input, out string? slug)
    {
        slug = null;
        try
        {
            slug = ValidSlug(input);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    //public static bool AreSame(Slug? s1, Slug? s2) => s1.HasValue && s2.HasValue && s1.Value == s2.Value;

    public static bool operator ==(Slug slug, string other) => slug.Value.Equals(other, StringComparison.OrdinalIgnoreCase);
    public static bool operator !=(Slug slug, string other) => !slug.Value.Equals(other, StringComparison.OrdinalIgnoreCase);

    //public static implicit operator DomainId(Slug input) => DomainId.Create(input);

    public static implicit operator string(Slug input) => input.Value;

    public static implicit operator Slug?(string? input) => TryParse(input, out Slug valid) ? valid : default(Slug?);

    public static implicit operator Slug(string input) => Parse(input);

    public override string ToString() => Value;

    public static bool TryParse(string? input, [NotNullWhen(true)] out Slug validSlug)
    {
        validSlug = Empty;
        if (input != null && IsValidSlug(input, out string? slug) && slug != null)
        {
            validSlug = Create(slug);
            return true;
        }
        return false;
    }

    public static Slug Parse(string value)
    {
        if (TryParse(value, out Slug result))
            return result;

        throw new ArgumentException($"Not a slug : {value}", nameof(value));
    }

    public int CompareTo(Slug other) => string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    public bool Equals(Slug other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    public static bool operator ==(Slug lhs, Slug rhs) => lhs.Equals(rhs);

    public static bool operator !=(Slug lhs, Slug rhs) => !lhs.Equals(rhs);
    public override bool Equals(object? obj)
    {
        if (obj is Slug slug)
            return Equals(slug);
        if (obj is string str)
            return string.Equals(Value, str, StringComparison.OrdinalIgnoreCase);
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase) + 19;
    public int CompareTo(object? obj) => CompareTo(Create(obj?.ToString() ?? string.Empty));

    public static Slug Parse(string s, IFormatProvider? _)
    {
        return Create(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? _, [MaybeNullWhen(false)] out Slug result)
    {
        return TryParse(s, out result);
    }
}
