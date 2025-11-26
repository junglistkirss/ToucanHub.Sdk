using System.Collections.Immutable;
using System.Diagnostics;

namespace ToucanHub.Sdk.Contracts.Security;

public readonly struct BitMask
{
    private readonly int _mask;

    public BitMask(int mask) => _mask = mask;

    public bool Intersects(BitMask other) => (_mask & other._mask) != 0;
}

[DebuggerDisplay("{Id,nq}")]
public readonly struct Permission : IComparable<Permission>, IEquatable<Permission>
{
    public const string Any = "*";
    public const string Exclude = "^";
    public const string Operation = "@";
    private const char OperationSeparator = ',';

    public static implicit operator string(Permission input) => input.Id;
    public static implicit operator Permission(string input) => new(input);

    public string Id { get; }
    public ImmutableArray<string> Operations { get; }
    public bool HasOperations => Operations.Length > 0;
    public string Scope => string.Join(Part.SeparatorMain, path.Select(x => x.ToString()));

    private readonly Part[] path;

    public Permission(string raw)
    {
        string scope = raw;
        string[] ops = [];

        int sep = raw.LastIndexOf(Operation);
        if (sep >= 0)
        {
            scope = raw[..sep];
            string opsRaw = raw[(sep + 1)..];

            ops = [..opsRaw
                .Split(OperationSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(o => o.ToLowerInvariant())
                .Distinct()
                ];
        }
        Id = raw;
        path = Part.ParsePath(scope) ?? [];
        Operations = [.. ops];
    }

    public bool Allows(Permission requested)
    {
        // 1️⃣ Vérification du scope
        Part[] lhs = this.path;
        Part[] rhs = requested.path;

        int lhsLen = lhs.Length;
        int rhsLen = rhs.Length;

        // this ne peut pas couvrir un scope plus profond
        if (lhsLen > rhsLen)
            return false;

        for (int i = 0; i < lhsLen; i++)
        {
            if (!Part.Intersects(ref lhs[i], ref rhs[i], allowNull: false))
                return false;
        }

        if (!requested.HasOperations)
            return true;

        if (!HasOperations)
            return false;

        bool AnyOperation = Operations.Length == 1 && Operations[0] == Any;
        if (AnyOperation)
            return true;

        var ops = Operations;
        var reqOps = requested.Operations;
        for (int i = 0; i < reqOps.Length; i++)
        {
            if (!ops.Contains(reqOps[i]))
                return false;
        }

        return true;
    }

    public bool Includes(Permission requested)
    {
        // 1️⃣ Comparaison sur le scope
        Part[] lhs = this.path;
        Part[] rhs = requested.path;

        int common = Math.Min(lhs.Length, rhs.Length);

        for (int i = 0; i < common; i++)
        {
            if (!Part.Intersects(ref lhs[i], ref rhs[i], allowNull: true))
                return false;
        }

        // Si le scope est compatible, l’inclusion logique est validée
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
