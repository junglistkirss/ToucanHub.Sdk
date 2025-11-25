using System.Diagnostics;

namespace ToucanHub.Sdk.Contracts.Names;

[DebuggerDisplay("{Type,nq} - {Identifier,nq}")]
public readonly struct ActorReference : IEquatable<ActorReference>, IComparable<ActorReference>
{
    public enum ActorType
    {
        Anonymous,
        User,
        Client,
        Machine,
    }

    public static readonly ActorReference Anonymous = new();

    private static readonly char[] TrimChars = { ' ', ':' };

    public ActorType Type { get; init; }

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

    public bool IsMachine => Type is ActorType.Machine;
    public bool IsClient => Type is ActorType.Client;
    public bool IsUser => Type is ActorType.User;
    public bool IsAnonymous => Type is ActorType.Anonymous;
    public bool IsEmpty => string.IsNullOrWhiteSpace(Identifier);

    public ActorReference()
    {
        Type = ActorType.Anonymous;
        Identifier = string.Empty;
    }

    public ActorReference(ActorType type, string? identifier)
    {
        Identifier = identifier?.Trim() ?? string.Empty;
        Type = type;
    }

    public static ActorReference Machine(string? identifier) => new(ActorType.Machine, identifier);
    
    public static ActorReference Client(string? identifier) => new(ActorType.Client, identifier);

    public static ActorReference User(string? identifier) => new(ActorType.User, identifier);

    public bool Equals(ActorReference other) => string.Equals(ToString(), other.ToString(), StringComparison.Ordinal);

    public override int GetHashCode() => ToString().GetHashCode(StringComparison.Ordinal);

    public int CompareTo(ActorReference other) => string.Compare(ToString(), other.ToString(), StringComparison.Ordinal);

    public static bool operator ==(ActorReference lhs, ActorReference rhs) => lhs.Equals(rhs);

    public static bool operator !=(ActorReference lhs, ActorReference rhs) => !lhs.Equals(rhs);

    public static implicit operator string(ActorReference input) => input.ToString();
    public static implicit operator string?(ActorReference? input) => input?.ToString();

    public override string ToString() => $"{Type}:{Identifier}";

    public static bool TryParse(string? value, out ActorReference result)
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
            if (!Enum.TryParse(value[..idx], true, out ActorType type))
                type = ActorType.User;

            result = new ActorReference(type, value[(idx + 1)..]);
        }
        else
        {
            result = new ActorReference(ActorType.User, value);
        }

        return true;
    }

    public static ActorReference Parse(string value)
    {
        if (!TryParse(value, out ActorReference result))
            throw new ArgumentException("Ref token cannot be null or empty.", nameof(value));

        return result;
    }

    public override bool Equals(object? obj) => obj is ActorReference token && Equals(token);
}

