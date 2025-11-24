namespace Toucan.Sdk.EventSourcing.Models;

public struct Versioning : IEquatable<Versioning>, IComparable<Versioning>
{
    public static readonly Versioning Any = From(-1);

    public static readonly Versioning Max = From(long.MaxValue);

    public static readonly Versioning Zero = From(default!);

    public Versioning() { }

    public static Versioning From(long raw)
    {
        return Zero with
        {
            Value = raw
        };
    }
    public long Value { get; private set; } = default!;

    public static bool operator >(Versioning v, long instance) => v.Value > instance;
    public static bool operator <(Versioning v, long instance) => v.Value < instance;

    public static bool operator >=(Versioning v, long instance) => v.Value >= instance;
    public static bool operator <=(Versioning v, long instance) => v.Value <= instance;
    public static Versioning operator ++(Versioning v) => v with { Value = v + 1 };

    [Obsolete("Avoid downgrade event version, events stream always increase. Please invent another patten for use case", true)]
    public static Versioning operator --(Versioning _) => throw new NotSupportedException("Event version cannot be downgradded");

    public static implicit operator long(Versioning input) => input.Value;

    public override readonly string ToString()
    {
        byte[] raw = BitConverter.GetBytes(Value);

        if (BitConverter.IsLittleEndian)
            raw = [.. raw.Reverse()];

        return BitConverter.ToString(raw).Replace("-", "");
    }
    public readonly bool Equals(Versioning other) => Value.Equals(other.Value);

    public override readonly int GetHashCode() => Value.GetHashCode();

    public readonly int CompareTo(Versioning other) => Value.CompareTo(other.Value);

    public static bool operator ==(Versioning lhs, Versioning rhs) => lhs.Equals(rhs);

    public static bool operator !=(Versioning lhs, Versioning rhs) => !lhs.Equals(rhs);

    public override readonly bool Equals(object? obj) => obj is Versioning versionning && Equals(versionning);
}

