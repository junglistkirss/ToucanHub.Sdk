using System.Diagnostics;

namespace ToucanHub.Sdk.Contracts.Names;

[DebuggerDisplay("{Type,nq} - {Identifier,nq}")]
public readonly struct Tenant : IEquatable<Tenant>, IComparable<Tenant>, IParsable<Tenant>
{
    public enum TenantType
    {
        Unspecified,
        Internal,
        External,
    }


    public static readonly Tenant Unspecified = new();

    private static readonly char[] TrimChars = [' ', ':'];

    public TenantType Type { get; init; } = TenantType.Unspecified;

    public string Identifier { get; init; } = string.Empty;

    public bool IsInternal => Type is TenantType.Internal;
    public bool IsExternal => Type is TenantType.External;
    public bool IsUnspecified => Type is TenantType.Unspecified;
    public bool IsEmpty => string.IsNullOrWhiteSpace(Identifier);

    public Tenant()
    {
        Type = TenantType.Unspecified;
        Identifier = string.Empty;
    }

    public Tenant(TenantType type, string? identifier)
    {
        Identifier = identifier?.Trim() ?? string.Empty;
        Type = type;
    }

    public static Tenant Internal(string? identifier) => new(TenantType.Internal, identifier);

    public static Tenant External(string? identifier) => new(TenantType.External, identifier);

    public bool Equals(Tenant other) => string.Equals(ToString(), other.ToString(), StringComparison.Ordinal);

    public override int GetHashCode() => ToString().GetHashCode(StringComparison.Ordinal);

    public int CompareTo(Tenant other) => string.Compare(ToString(), other.ToString(), StringComparison.Ordinal);

    public static bool operator ==(Tenant lhs, Tenant rhs) => lhs.Equals(rhs);

    public static bool operator !=(Tenant lhs, Tenant rhs) => !lhs.Equals(rhs);

    public static implicit operator string(Tenant input) => input.ToString();
    public static implicit operator string?(Tenant? input) => input?.ToString();

    public override string ToString() => $"{Type.ToString().ToLowerInvariant()}:{Identifier}";

    public static bool TryParse(string? value, out Tenant result)
    {
        value = value?.Trim(TrimChars);

        if (string.IsNullOrWhiteSpace(value))
        {
            result = Unspecified;
            return false;
        }

        value = value.Trim();

        int idx = value.IndexOf(':', StringComparison.Ordinal);

        if (idx > 0 && idx < value.Length - 1)
        {
            if (!Enum.TryParse(value[..idx], true, out TenantType type))
                type = TenantType.External;

            result = new Tenant(type, value[(idx + 1)..]);
        }
        else
        {
            result = new Tenant(TenantType.External, value);
        }

        return true;
    }

    public static Tenant Parse(string value)
    {
        if (!TryParse(value, out Tenant result))
            throw new ArgumentException("Ref token cannot be null or empty.", nameof(value));

        return result;
    }

    public override bool Equals(object? obj) => obj is Tenant tenant && Equals(tenant);

    public static Tenant Parse(string s, IFormatProvider? _)
    {
        return Parse(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? _, [MaybeNullWhen(false)] out Tenant result)
    {
        return TryParse(s, out result);
    }
}

