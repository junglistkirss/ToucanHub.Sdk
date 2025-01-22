using System.Diagnostics;

namespace Toucan.Sdk.Contracts.Names;

[DebuggerDisplay("{Name,nq}")]
public readonly struct Tag(string Name) : IEquatable<Tag>, IComparable<Tag>, IParsable<Tag>
{
    public static readonly Tag Empty = string.Empty; // space is important to bypass guard, spaces only is always mapped to Empty
    public string Name { get; } = ValidTag(Name);

    public static bool operator ==(Tag tag, string? other) => tag.Name.Equals(other, StringComparison.OrdinalIgnoreCase);
    public static bool operator !=(Tag tag, string? other) => !tag.Name.Equals(other, StringComparison.OrdinalIgnoreCase);

    public static implicit operator string(Tag input) => input.Name;

    public static implicit operator Tag(string input) => Parse(input);

    public override string ToString() => Name;

    public static string ValidTag(string input)
    {

        if (input == null || string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.Trim();

        if (NamingConvention.TagRegex().IsMatch(input))
            return input;
        throw new InvalidDataException($"{nameof(input)} is not a valid tag : {input}");
    }

    public static bool IsValidTag(string input) => IsValidTag(input, out string? _);
    public static bool IsValidTag(string input, out string? tag)
    {
        tag = null;
        try
        {
            tag = ValidTag(input);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool TryParse(string? input, [NotNullWhen(true)] out Tag validTag)
    {
        validTag = Empty;
        if (input != null && IsValidTag(input, out string? tag) && tag != null)
        {
            validTag = new Tag(tag);
            return true;
        }
        return false;
    }

    public static Tag Parse(string value)
    {
        if (TryParse(value, out Tag result))
            return result;
        throw new ArgumentException("Not a Tag", nameof(value));
    }

    public int CompareTo(Tag other) => StringComparer.OrdinalIgnoreCase.Compare(Name, other.Name);

    public bool Equals(Tag other)
    {
        return Name.Equals(other, StringComparison.OrdinalIgnoreCase);
    }

    public static Tag Parse(string s, IFormatProvider? provider)
    {
        return new(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Tag result)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Tag tag)
            return Equals(tag);
        if (obj is string str)
            return Name.Equals(str, StringComparison.OrdinalIgnoreCase);
        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode(StringComparison.OrdinalIgnoreCase) + 17;
    }

    public static bool operator <(Tag left, Tag right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Tag left, Tag right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Tag left, Tag right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Tag left, Tag right)
    {
        return left.CompareTo(right) >= 0;
    }
}