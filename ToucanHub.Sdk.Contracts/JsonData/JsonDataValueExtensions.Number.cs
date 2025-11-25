using System.Globalization;

namespace ToucanHub.Sdk.Contracts.JsonData;

public static partial class JsonDataValueExtensions
{

    public static bool IsNumber<T>(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out T result)
        where T : unmanaged
    {
        result = default;

        if (jsonDataValue.Type == JsonDataValueType.Number && jsonDataValue.RawValue is T typedValue)
        {
            result = typedValue;
            return true;
        }
        return false;
    }
    public static T AsNumber<T>(this JsonDataValue jsonDataValue)
        where T : unmanaged
    {
        if (jsonDataValue.IsNumber(out T res))
            return res;
        throw new InvalidCastException();
    }
    public static bool IsByte(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out byte result)
    {
        result = default;
        if (jsonDataValue.RawValue is byte b)
        {
            result = b;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToByte(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && byte.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out byte value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static byte AsByte(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsByte(out byte value))
            return value;
        throw new InvalidCastException();
    }
    public static bool IsSByte(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out sbyte result)
    {
        result = default;
        if (jsonDataValue.RawValue is sbyte typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToSByte(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && sbyte.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out sbyte value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static sbyte AsSByte(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsSByte(out sbyte value))
            return value;
        throw new InvalidCastException();
    }
    public static bool IsShort(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out short result)
    {
        result = default;
        if (jsonDataValue.RawValue is short typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToInt16(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && short.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out short value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static short AsShort(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsShort(out short value))
            return value;
        throw new InvalidCastException();
    }
    public static bool IsUShort(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out ushort result)
    {
        result = default;
        if (jsonDataValue.RawValue is ushort typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToUInt16(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && ushort.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out ushort value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static ushort AsUShort(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsUShort(out ushort value))
            return value;
        throw new InvalidCastException();
    }
    public static bool IsInt(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out int result)
    {
        result = default;
        if (jsonDataValue.RawValue is int typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToInt32(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && int.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out int value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static int AsInt(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsInt(out int value))
            return value;
        throw new InvalidCastException();
    }
    public static bool IsUInt(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out uint result)
    {
        result = default;
        if (jsonDataValue.RawValue is uint typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToUInt32(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && uint.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out uint value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static uint AsUInt(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsUInt(out uint value))
            return value;
        throw new InvalidCastException();
    }
    public static bool IsLong(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out long result)
    {
        result = default;
        if (jsonDataValue.RawValue is long typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToInt64(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && long.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out long value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static long AsLong(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsLong(out long value))
            return value;
        throw new InvalidCastException();
    }
    public static bool IsFloat(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out float result)
    {
        result = default;
        if (jsonDataValue.RawValue is float typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToSingle(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && float.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out float value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static float AsFloat(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsFloat(out float value))
            return value;
        throw new InvalidCastException();
    }
    public static bool IsDouble(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out double result)
    {
        result = default;
        if (jsonDataValue.RawValue is double typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToDouble(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && double.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out double value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static double AsDouble(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsDouble(out double value))
            return value;
        throw new InvalidCastException();
    }
    public static bool IsDecimal(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out decimal result)
    {
        result = default;
        if (jsonDataValue.RawValue is decimal typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToDecimal(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && decimal.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out decimal value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static decimal AsDecimal(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsDecimal(out decimal value))
            return value;
        throw new InvalidCastException();
    }
    public static bool IsULong(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out ulong result)
    {
        result = default;
        if (jsonDataValue.RawValue is ulong typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Number)
        {
            result = Convert.ToUInt64(jsonDataValue.RawValue);
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.String && ulong.TryParse(jsonDataValue.RawValue?.ToString(), CultureInfo.InvariantCulture, out ulong value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public static ulong AsULong(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsULong(out ulong value))
            return value;
        throw new InvalidCastException();
    }
}
