using System.Globalization;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.JsonData;

public static partial class JsonDataValueExtensions
{

    public static bool IsString(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out string? result)
    {
        result = null;
        if (jsonDataValue.RawValue is string typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String)
        {
            result = jsonDataValue.RawValue?.ToString()!;
            return true;
        }
        return false;
    }
    public static string? AsString(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsString(out string? str))
            return str;
        throw new InvalidCastException();
    }
    public static bool IsBinary(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out byte[]? result)
    {
        result = null;
        if (jsonDataValue.RawValue is byte[] typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.IsString(out string? str))
        {
            result = Convert.FromBase64String(str);
            return true;
        }
        return false;
    }
    public static byte[]? AsBinary(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsBinary(out byte[]? str))
            return str;
        throw new InvalidCastException();
    }
    public static bool IsGuid(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out Guid result)
    {
        result = Guid.Empty;

        if (jsonDataValue.RawValue is Guid gid)
        {
            result = gid;
            return true;
        }
        else if (Guid.TryParse(jsonDataValue.RawValue?.ToString(), out Guid id))
        {
            result = id;
            return true;
        }
        return false;
    }
    public static bool IsDomainId(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out DomainId result)
    {
        result = DomainId.Empty;

        if (jsonDataValue.RawValue is DomainId did)
        {
            result = did;
            return true;
        }
        else if (DomainId.TryParse(jsonDataValue.RawValue?.ToString(), out DomainId id))
        {
            result = id;
            return true;
        }
        return false;
    }
    public static bool IsSlug(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out Slug result)
    {
        result = Slug.Empty;

        if (jsonDataValue.RawValue is Slug s)
        {
            result = s;
            return true;
        }
        else if (Slug.TryParse(jsonDataValue.RawValue?.ToString(), out Slug id))
        {
            result = id;
            return true;
        }
        return false;
    }
    public static bool IsDateTime(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out DateTime result)
    {
        result = default!;

        if (jsonDataValue.RawValue is DateTime typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && DateTime.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out DateTime value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static DateTime AsDateTime(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsDateTime(out DateTime res))
            return res;
        throw new InvalidCastException();
    }
    public static bool IsDateTimeOffset(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out DateTimeOffset result)
    {
        result = default!;

        if (jsonDataValue.RawValue is DateTimeOffset typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && DateTimeOffset.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out DateTimeOffset value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static DateTimeOffset AsDateTimeOffset(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsDateTimeOffset(out DateTimeOffset res))
            return res;
        throw new InvalidCastException();
    }
    public static bool IsTimeSpan(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out TimeSpan result)
    {
        result = default!;

        if (jsonDataValue.RawValue is TimeSpan typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && TimeSpan.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out TimeSpan value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static TimeSpan AsTimeSpan(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsTimeSpan(out TimeSpan res))
            return res;
        throw new InvalidCastException();
    }
    public static bool IsDateOnly(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out DateOnly result)
    {
        result = default!;

        if (jsonDataValue.RawValue is DateOnly typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && DateOnly.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out DateOnly value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static DateOnly AsDateOnly(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsDateOnly(out DateOnly res))
            return res;
        throw new InvalidCastException();
    }
    public static bool IsTimeOnly(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out TimeOnly result)
    {
        result = default!;

        if (jsonDataValue.RawValue is TimeOnly typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && TimeOnly.TryParse(jsonDataValue.RawValue!.ToString(), CultureInfo.InvariantCulture, out TimeOnly value))
        {
            result = value;
        }
        return false;
    }
    public static TimeOnly AsTimeOnly(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsTimeOnly(out TimeOnly res))
            return res;
        throw new InvalidCastException();
    }
}
