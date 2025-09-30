using System.Diagnostics;

namespace Toucan.Sdk.Contracts.Security;

[DebuggerDisplay("{Name,nq}")]
public readonly struct Realm : IComparable<Realm>, IEquatable<Realm>
{
    public static implicit operator string(Realm input) => input.Name;
    public static implicit operator Realm(string input) => new(input);
    public Realm(string name)
    {
        Name = name;
    }
    public readonly string Name { get; }

    public int CompareTo(Realm other) => string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    public bool Equals(Realm other) => string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj is Realm scope && Equals(scope);

    public static bool operator ==(Realm left, Realm right) => left.Equals(right);

    public static bool operator !=(Realm left, Realm right) => !(left == right);

    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase) * 19;

    public static bool operator <(Realm left, Realm right) => left.CompareTo(right) < 0;

    public static bool operator <=(Realm left, Realm right) => left.CompareTo(right) <= 0;

    public static bool operator >(Realm left, Realm right) => left.CompareTo(right) > 0;

    public static bool operator >=(Realm left, Realm right) => left.CompareTo(right) >= 0;
    public override string ToString()
    {
        return Name;
    }
}
