namespace Toucan.Sdk.Utils;

public static class EnumExtensions
{
    public static bool IsEnumValue<TEnum>(this object value) where TEnum : struct
    {
        try
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }
        catch
        {
            return false;
        }
    }

    public static bool TryConvert<TEnum>(this string? src, out TEnum enumValue, TEnum defaultValue = default, bool ignoreCase = true)
        where TEnum : struct
    {
        enumValue = defaultValue;
        return !string.IsNullOrWhiteSpace(src) && Enum.TryParse(src, ignoreCase, out enumValue);
    }

    public static bool TryConvertOrNull<TEnum>(this string? src, out TEnum? sct, TEnum? defaultValue = null, bool ignoreCase = true)
        where TEnum : struct
    {
        sct = defaultValue;
        if (!string.IsNullOrWhiteSpace(src) && Enum.TryParse(src, ignoreCase, out TEnum val))
        {
            sct = val;
            return true;
        }
        return false;
    }

    public static TEnum Convert<TEnum>(this string? src, TEnum defaultValue = default, bool ignoreCase = true)
        where TEnum : struct => !string.IsNullOrWhiteSpace(src) && Enum.TryParse(src, ignoreCase, out TEnum enumValue) ? enumValue : defaultValue;

    public static TEnum? ConvertOrNull<TEnum>(this string? src, bool ignoreCase = true)
        where TEnum : struct => !string.IsNullOrWhiteSpace(src) && Enum.TryParse(src, ignoreCase, out TEnum enumValue) ? enumValue : null;

    public static TEnum Convert<TEnum>(this object? src, TEnum defaultValue = default, bool ignoreCase = true)
       where TEnum : struct => src is not null && Enum.TryParse(src.ToString(), ignoreCase, out TEnum enumValue) ? enumValue : defaultValue;

    public static TEnum? ConvertOrNull<TEnum>(this object? src, bool ignoreCase = true)
        where TEnum : struct => src is not null && Enum.TryParse(src.ToString(), ignoreCase, out TEnum enumValue) ? enumValue : null;
}

