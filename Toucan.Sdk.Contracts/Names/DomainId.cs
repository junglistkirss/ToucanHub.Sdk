using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Toucan.Sdk.Contracts.Names;

[DebuggerDisplay("{Id,nq}")]
public readonly struct DomainId : IEquatable<DomainId>, IComparable<DomainId>, IParsable<DomainId>
{
    private static readonly string EmptyRawValueId = Guid.Empty.ToString("N");
    public static readonly DomainId Empty = new(EmptyRawValueId!);
    public static readonly char IdSeparator = '~';

    public string Id => Parts != null ? string.Join(IdSeparator, Parts) : EmptyRawValueId;
    public string[] Parts { get; }
    private DomainId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException($"'{nameof(id)}' ne peut pas avoir une valeur null ou être un espace blanc.", nameof(id));

        Parts = id.Trim().ToLowerInvariant().Split(IdSeparator, StringSplitOptions.RemoveEmptyEntries);
    }

    public static bool TryParse(string? input, [NotNullWhen(true)] out DomainId validDomainId)
    {
        validDomainId = Empty;
        if (string.IsNullOrWhiteSpace(input) || string.Equals(input, EmptyRawValueId, StringComparison.OrdinalIgnoreCase))
            return false;

        DomainId resp = Empty;
        foreach (string item in input.Trim(' ', IdSeparator).Split(IdSeparator))
        {
            if (Guid.TryParse(item, out Guid uid) && uid != Guid.Empty)
                resp = Combine(resp, FromGuid(uid));
            else if (Slug.TryParse(item, out Slug slug) && slug != Slug.Empty)
                resp = Combine(resp, FromSlug(slug));
            else return false;
        }
        validDomainId = resp;
        return true;
    }

    public static DomainId From(params Slug[] values)
    {
        DomainId response = Empty;
        foreach (Slug item in values)
        {
            response = Combine(response, FromSlug(item));
        }
        return response;
    }

    public static DomainId From(params DomainId[] values)
    {
        DomainId response = Empty;
        foreach (DomainId item in values)
        {
            response = Combine(response, item);
        }
        return response;
    }


    public static DomainId Parse(string value)
    {
        if (TryParse(value, out DomainId valid))
            return valid;
        return Empty;
    }

    internal static DomainId FromSlug(Slug value)
    {
        if (string.Equals(value, EmptyRawValueId, StringComparison.OrdinalIgnoreCase))
            return Empty;

        return new DomainId(value);
    }

    public static DomainId FromGuid(Guid value)
    {
        if (value == Guid.Empty)
            return Empty;

        return new DomainId(value.ToString("N"));
    }

    public override bool Equals(object? obj) => obj is DomainId status && Equals(status);

    public bool Equals(DomainId other) => string.Equals(ToString(), other.ToString(), StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => ToString().GetHashCode(StringComparison.OrdinalIgnoreCase) + 3;

    public override string ToString() => Id!;

    public int CompareTo(DomainId other) => string.Compare(ToString(), other.ToString(), StringComparison.OrdinalIgnoreCase);

    public static bool operator ==(DomainId lhs, DomainId rhs) => lhs.Equals(rhs);

    public static bool operator !=(DomainId lhs, DomainId rhs) => !lhs.Equals(rhs);

    // public static implicit operator DomainId(Slug input) => new(input);


    //public static implicit operator Guid(DomainId input)
    //{
    //    return input.RequireGuid();
    //}


    public static implicit operator string(DomainId input) => input.ToString();

    public static implicit operator DomainId(Guid input) => FromGuid(input);
    
    public static DomainId New() => new(Guid.NewGuid().ToString("N"));

    [Pure]
    public DomainId With(params DomainId[] next)
    {
        DomainId n = this;
        foreach (DomainId item in next)
        {
            n = Combine(n, item);
        }
        return n;
    }

    [Pure]
    public DomainId With(params Slug[] next)
    {
        DomainId n = this;
        foreach (Slug item in next)
        {
            n = Combine(n, FromSlug(item));
        }
        return n;
    }
    public static DomainId Combine(params DomainId[] ids)
    {
        var next = Empty;
        foreach (var id in ids)
            next = Combine2(next, id);
        return next;
    }
    private static DomainId Combine2(DomainId id1, DomainId id2)
    {
        if (id1 == Empty)
            return id2;
        if (id2 == Empty)
            return id1;
        return new($"{id1}{IdSeparator}{id2}");
    }

    public static bool TryParseGuid(DomainId? value, [NotNullWhen(true)] out Guid? uid)
    {
        uid = null;

        if (value != Empty && Guid.TryParse(value.ToString(), out Guid uuid))
        {
            uid = uuid;
            return true;
        }
        return false;
    }

    public static Guid ParseGuid(DomainId? value)
    {
        if (TryParseGuid(value, out Guid? result) && result.HasValue)
            return result.Value;

        throw new ArgumentException("Cannot translate to UUID", nameof(value));
    }

    public static bool TryParseSlug(DomainId value, [NotNullWhen(true)] out Slug slug)
    {
        slug = Slug.Empty;

        if (value != Empty && Slug.TryParse(value.ToString(), out Slug name))
        {
            slug = name;
            return true;
        }
        return false;
    }

    public static Slug ParseSlug(DomainId value)
    {
        if (TryParseSlug(value, out Slug result))
            return result;

        throw new ArgumentException("Cannot translate to SLUG", nameof(value));
    }

    public static DomainId Parse(string s, IFormatProvider? provider)
    {
        return Parse(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out DomainId result)
    {
        return TryParse(s, out result);
    }

    public static bool operator <(DomainId left, DomainId right) => left.CompareTo(right) < 0;

    public static bool operator <=(DomainId left, DomainId right) => left.CompareTo(right) <= 0;

    public static bool operator >(DomainId left, DomainId right) => left.CompareTo(right) > 0;

    public static bool operator >=(DomainId left, DomainId right) => left.CompareTo(right) >= 0;
}

