using System.Diagnostics;

namespace Toucan.Sdk.Contracts.Security;

[DebuggerDisplay("{Name,nq}")]
public readonly struct AppScope : IComparable<AppScope>, IEquatable<AppScope>
{
    public static implicit operator string(AppScope input) => input.Name;
    public static implicit operator AppScope(string input) => new(input);
    public AppScope(string name)
    {
        Name = name;
    }
    public readonly string Name { get; }

    public int CompareTo(AppScope other) => string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    public bool Equals(AppScope other) => string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj is AppScope scope && Equals(scope);

    public static bool operator ==(AppScope left, AppScope right) => left.Equals(right);

    public static bool operator !=(AppScope left, AppScope right) => !(left == right);

    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase) * 19;

    public static bool operator <(AppScope left, AppScope right) => left.CompareTo(right) < 0;

    public static bool operator <=(AppScope left, AppScope right) => left.CompareTo(right) <= 0;

    public static bool operator >(AppScope left, AppScope right) => left.CompareTo(right) > 0;

    public static bool operator >=(AppScope left, AppScope right) => left.CompareTo(right) >= 0;
    public override string ToString()
    {
        return Name;
    }
}
