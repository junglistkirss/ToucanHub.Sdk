namespace Toucan.Sdk.Contracts.JsonData;

public static partial class JsonDataValueExtensions
{
    public static bool IsNull(this JsonDataValue jsonDataValue)
    {
        return jsonDataValue.Type == JsonDataValueType.Null || jsonDataValue.RawValue is null;
    }

    public static bool IsBoolean(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out bool result)
    {
        result = default;
        if (jsonDataValue.RawValue is bool typed)
        {
            result = typed;
            return true;
        }
        if (jsonDataValue.Type == JsonDataValueType.Boolean)
        {
            result = Convert.ToBoolean(jsonDataValue.RawValue);
            return true;
        }
        else if (bool.TryParse(jsonDataValue.RawValue?.ToString(), out bool value))
        {
            result = value;
            return true;
        }
        return false;
    }

    public static bool AsBoolean(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsBoolean(out bool value))
            return value;
        throw new InvalidCastException();
    }


    public static bool IsArray(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out JsonDataArray result)
    {
        result = default!;
        if (jsonDataValue.RawValue is JsonDataArray typed)
        {
            result = typed;
            return true;
        }
        return false;
    }
    public static JsonDataArray AsArray(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsArray(out JsonDataArray res))
            return res;
        throw new InvalidCastException();
    }

    public static bool IsObject(this JsonDataValue jsonDataValue, [NotNullWhen(true)] out JsonDataObject result)
    {
        result = default!;
        if (jsonDataValue.RawValue is JsonDataObject typed)
        {
            result = typed;
            return true;
        }
        return false;
    }

    public static JsonDataObject AsObject(this JsonDataValue jsonDataValue)
    {
        if (jsonDataValue.IsObject(out JsonDataObject res))
            return res;
        throw new InvalidCastException();
    }
}
