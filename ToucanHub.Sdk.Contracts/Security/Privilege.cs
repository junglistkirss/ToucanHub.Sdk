using System.Diagnostics;

namespace ToucanHub.Sdk.Contracts.Security;

[DebuggerDisplay("{Value,nq}")]
public readonly struct Privilege(string value)
{
    public string Value { get; } = value;
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if(obj is Privilege privilege) 
            return string.Equals(Value, privilege.Value, StringComparison.OrdinalIgnoreCase);
        return false;
    }

    public override string ToString()
    {
        return Value;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    public static bool operator ==(Privilege left, Privilege right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Privilege left, Privilege right)
    {
        return !(left == right);
    }
}
