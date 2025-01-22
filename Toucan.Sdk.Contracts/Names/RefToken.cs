using System.Diagnostics;

namespace Toucan.Sdk.Contracts.Names;

[DebuggerDisplay("{Type,nq} - {Identifier,nq}")]
public readonly struct RefToken : IEquatable<RefToken>, IComparable<RefToken>
{
    public enum RefTokenType
    {
        Anonymous,
        User,
        Client,
    }

    public static readonly RefToken Anonymous = new();

    private static readonly char[] TrimChars = { ' ', ':' };

    public RefTokenType Type { get; init; }

    public string Identifier { get; init; }
    public string? Display
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Identifier))
                return null;
            int spaceIndex = Identifier.IndexOf(' ');
            if (spaceIndex > 0)
                return Identifier.Substring(0, spaceIndex);
            return Identifier;
        }
    }

    public bool IsClient => Type == RefTokenType.Client;
    public bool IsUser => Type == RefTokenType.User;
    public bool IsAnonymous => Type == RefTokenType.Anonymous;
    public bool IsEmpty => string.IsNullOrWhiteSpace(Identifier);

    public RefToken()
    {
        Type = RefTokenType.Anonymous;
        Identifier = string.Empty;
    }

    public RefToken(RefTokenType type, string? identifier)
    {
        Identifier = identifier?.Trim() ?? string.Empty;
        Type = type;
    }

    public static RefToken Client(string? identifier) => new(RefTokenType.Client, identifier);

    public static RefToken User(string? identifier) => new(RefTokenType.User, identifier);

    public bool Equals(RefToken other) => string.Equals(ToString(), other.ToString(), StringComparison.Ordinal);

    public override int GetHashCode() => ToString().GetHashCode(StringComparison.Ordinal);

    public int CompareTo(RefToken other) => string.Compare(ToString(), other.ToString(), StringComparison.Ordinal);

    public static bool operator ==(RefToken lhs, RefToken rhs) => lhs.Equals(rhs);

    public static bool operator !=(RefToken lhs, RefToken rhs) => !lhs.Equals(rhs);

    public static implicit operator string(RefToken input) => input.ToString();
    public static implicit operator string?(RefToken? input) => input?.ToString();

    public override string ToString() => $"{Type}:{Identifier}";

    public static bool TryParse(string? value, out RefToken result)
    {
        value = value?.Trim(TrimChars);

        if (string.IsNullOrWhiteSpace(value))
        {
            result = Anonymous;
            return false;
        }

        value = value.Trim();

        int idx = value.IndexOf(':', StringComparison.Ordinal);

        if (idx > 0 && idx < value.Length - 1)
        {
            if (!Enum.TryParse(value[..idx], true, out RefTokenType type))
                type = RefTokenType.User;

            result = new RefToken(type, value[(idx + 1)..]);
        }
        else
        {
            result = new RefToken(RefTokenType.User, value);
        }

        return true;
    }

    public static RefToken Parse(string value)
    {
        if (!TryParse(value, out RefToken result))
            throw new ArgumentException("Ref token cannot be null or empty.", nameof(value));

        return result;
    }

    public override bool Equals(object? obj) => obj is RefToken token && Equals(token);
}

