using System.Diagnostics;

namespace Toucan.Sdk.Contracts.Security;

[DebuggerDisplay("{Id,nq}")]
public readonly struct Permission : IComparable<Permission>, IEquatable<Permission>
{
    public const string Any = "*";
    public const string Exclude = "^";

    public static implicit operator string(Permission input) => input.Id;
    public static implicit operator Permission(string input) => new(input);

    public string Id { get; }

    private readonly Part[] path;

    public Permission(string id)
    {
        Id = id;
        path = Part.ParsePath(Id) ?? default!;
    }

    public bool Allows(Permission permission) => Covers(path, permission.path);

    public bool Includes(Permission permission) => PartialCovers(path, permission.path);

    private static bool Covers(Part[] given, Part[] requested)
    {
        if (given.Length > requested.Length)
            return false;

        for (int i = 0; i < given.Length; i++)
        {
            if (!Part.Intersects(ref given[i], ref requested[i], false))
                return false;
        }

        return true;
    }

    private static bool PartialCovers(Part[] given, Part[] requested)
    {
        for (int i = 0; i < Math.Min(given.Length, requested.Length); i++)
        {
            if (!Part.Intersects(ref given[i], ref requested[i], true))
                return false;
        }

        return true;
    }

    public bool StartsWith(string test) => Id.StartsWith(test, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj is Permission permission && Equals(permission);

    public bool Equals(Permission other) => other.Id.Equals(Id, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => Id.GetHashCode(StringComparison.OrdinalIgnoreCase) * 17;

    public override string ToString() => Id;

    public int CompareTo(Permission other) => string.Compare(Id, other.Id, StringComparison.OrdinalIgnoreCase);

    public static bool operator ==(Permission left, Permission right) => left.Equals(right);

    public static bool operator !=(Permission left, Permission right) => !(left == right);

    public static bool operator <(Permission left, Permission right) => left.CompareTo(right) < 0;

    public static bool operator <=(Permission left, Permission right) => left.CompareTo(right) <= 0;

    public static bool operator >(Permission left, Permission right) => left.CompareTo(right) > 0;

    public static bool operator >=(Permission left, Permission right) => left.CompareTo(right) >= 0;
}
