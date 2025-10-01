using System.Diagnostics;

namespace Toucan.Sdk.Contracts.Security;

[DebuggerDisplay("{Name,nq}")]
public readonly struct Area(string name) : IComparable<Area>, IEquatable<Area>
{
    public static implicit operator string(Area input) => input.Name;
    public static implicit operator Area(string input) => new(input);

    public readonly string Name { get; } = name;

    public int CompareTo(Area other) => string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    public bool Equals(Area other) => string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj is Area scope && Equals(scope);

    public static bool operator ==(Area left, Area right) => left.Equals(right);

    public static bool operator !=(Area left, Area right) => !(left == right);

    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase) * 19;

    public static bool operator <(Area left, Area right) => left.CompareTo(right) < 0;

    public static bool operator <=(Area left, Area right) => left.CompareTo(right) <= 0;

    public static bool operator >(Area left, Area right) => left.CompareTo(right) > 0;

    public static bool operator >=(Area left, Area right) => left.CompareTo(right) >= 0;
    public override string ToString()
    {
        return Name;
    }
}
