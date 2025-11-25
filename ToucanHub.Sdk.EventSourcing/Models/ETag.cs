using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ToucanHub.Sdk.EventSourcing.Models;

public readonly struct ETag(byte[] value) : IEquatable<ETag>, IParsable<ETag>, IEnumerable<byte>
{
    public static readonly ETag Empty = new([]);

    private readonly byte[] value = value;

    public static ETag Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, null, out ETag result))
            return result;

        throw new ArgumentException("Not an ETag", nameof(s));
    }

    public static bool TryParse(string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ETag result)
    {
        result = Empty;

        if (string.IsNullOrWhiteSpace(s))
            return false;

        result = new ETag(Encoding.UTF8.GetBytes(s));
        return true;
    }

    public bool Equals(ETag other)
    {
        if (ReferenceEquals(value, other.value))
            return true;

        if (other.value.Length != value.Length)
            return false;

        return value.SequenceEqual(other.value);
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return value.AsEnumerable().GetEnumerator();
    }

    public override int GetHashCode()
    {
        return value.GetHashCode() + 3;
    }

    public override string ToString()
    {
        return Convert.ToHexString(value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return value.GetEnumerator();
    }

    public static implicit operator string(ETag input) => input.ToString();
    public static implicit operator ETag(string input) => new(Convert.FromHexString(input));
    public static implicit operator byte[](ETag input) => input.value;
    public static implicit operator ETag(byte[] input) => new(input);

    public override bool Equals(object? obj)
    {
        return obj is ETag t && Equals(t);
    }

    public static bool operator ==(ETag left, ETag right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ETag left, ETag right)
    {
        return !(left == right);
    }
}
