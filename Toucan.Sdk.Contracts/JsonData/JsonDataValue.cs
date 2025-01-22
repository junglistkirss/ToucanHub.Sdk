using System.Globalization;
using System.Text.Json;
using Toucan.Sdk.Contracts.Extensions;
using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.JsonData;

public readonly struct JsonDataValue : IEquatable<JsonDataValue>, ITarget<object?>
{

    public static readonly JsonDataValue Null;
    public static readonly JsonDataValue True = new(true);
    public static readonly JsonDataValue False = new(false);
    public static readonly JsonDataValue Zero = new(0.0d);

    private readonly object? _value;

    public JsonDataValue(double value)
    {
        _value = value;
    }

    public JsonDataValue(bool value)
    {
        _value = value;
    }

    public JsonDataValue(string? value)
    {
        _value = value;
    }

    public JsonDataValue(JsonDataArray? value)
    {
        _value = value;
    }

    public JsonDataValue(JsonDataObject? value)
    {
        _value = value;
    }

    public JsonDataValueType Type
    {
        get
        {
            return _value switch
            {
                null => JsonDataValueType.Null,
                bool => JsonDataValueType.Boolean,
                double => JsonDataValueType.Number,
                string => JsonDataValueType.String,
                JsonDataArray => JsonDataValueType.Array,
                JsonDataObject => JsonDataValueType.Object,
                _ => throw new InvalidOperationException("Type is invalid"),
            };
        }
    }

    public bool IsNull()
    {
        return Type == JsonDataValueType.Null;
    }

    public bool IsBoolean([NotNullWhen(true)] out bool? result)
    {
        result = null;

        if (Type == JsonDataValueType.Boolean)
        {
            result = AsBoolean();
            return true;
        }
        return false;
    }

    public bool AsBoolean()
    {
        if (_value is bool typed)
        {
            return typed;
        }

        ThrowInvalidType();
        return default!;
    }

    public bool IsNumber([NotNullWhen(true)] out double? result)
    {
        result = null;

        if (Type == JsonDataValueType.Number)
        {
            result = AsNumber();
            return true;
        }
        return false;
    }

    public double AsNumber()
    {
        if (_value is double typed)
        {
            return typed;
        }

        ThrowInvalidType();
        return default!;
    }

    public bool IsString([NotNullWhen(true)] out string? result)
    {
        result = null;

        if (Type == JsonDataValueType.String)
        {
            result = AsString();
            return true;
        }
        return false;
    }

    public string AsString()
    {
        if (_value is string typed)
        {
            return typed;
        }

        ThrowInvalidType();
        return default!;
    }

    public bool IsDomainId([NotNullWhen(true)] out DomainId? result)
    {
        result = null;

        if (Type == JsonDataValueType.String)
        {
            if (DomainId.TryParse(AsString(), out DomainId id))
            {
                result = id;
                return true;
            }
        }
        return false;
    }

    public bool IsDateTime([NotNullWhen(true)] out DateTime? result)
    {
        result = null;

        if (Type == JsonDataValueType.String)
        {
            if (DateTime.TryParse(AsString(), CultureInfo.InvariantCulture, out DateTime id))
            {
                result = id;
                return true;
            }
        }
        return false;
    }
    public bool IsArray([NotNullWhen(true)] out JsonDataArray? result)
    {
        result = null;

        if (Type == JsonDataValueType.Array)
        {
            result = AsArray();
            return true;
        }
        return false;
    }
    public JsonDataArray AsArray()
    {
        if (_value is JsonDataArray typed)
        {
            return typed;
        }

        ThrowInvalidType();
        return default!;
    }

    public bool IsObject([NotNullWhen(true)] out JsonDataObject? result)
    {
        result = null;

        if (Type == JsonDataValueType.Object)
        {
            result = AsObject();
            return true;
        }
        return false;
    }

    public JsonDataObject AsObject()
    {
        if (_value is JsonDataObject typed)
        {
            return typed;
        }

        ThrowInvalidType();
        return default!;
    }


    private static JsonDataValue Parse(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.False:
                return False;
            case JsonValueKind.True:
                return True;
            case JsonValueKind.Number:
                return new(element.GetDouble());
            case JsonValueKind.String:
                return new(element.GetString());
            case JsonValueKind.Null:
                return Null;
            case JsonValueKind.Object:
                JsonDataObject obj = [];

                foreach (JsonProperty property in element.EnumerateObject())
                {
                    obj.Add(property.Name, Parse(property.Value));
                }

                return new(obj);
            case JsonValueKind.Array:
                JsonDataArray arr = [];

                foreach (JsonElement item in element.EnumerateArray())
                {
                    arr.Add(Parse(item));
                }

                return new(arr);
            default:
                throw new NotSupportedException();
        }
    }

    public static JsonDataValue Create(object? inputValue)
    {
        if (inputValue == null)
            return Null;

        if (inputValue is JsonDataValue v)
            return v;

        return inputValue switch
        {
            Guid typed => typed.ToString(),
            DomainId typed => typed,
            DateTime typed => typed.ToString(null, CultureInfo.InvariantCulture),
            bool typed => typed,
            float typed => (double)typed,
            double typed => (double)typed,
            int typed => (double)typed,
            long typed => (double)typed,
            string typed => typed,

            JsonDataArray typed => typed,
            JsonDataObject typed => typed,

            JsonElement typed => Parse(typed),
            JsonDocument typed => Parse(typed.RootElement),

            DomainId[] typed => Array(typed),
            IEnumerable<JsonDataObject> typed => Array(typed),
            IReadOnlyDictionary<string, object?> typed => Object(typed),
            IEnumerable<KeyValuePair<string, object?>> typed => Array(typed),
            object[] typed => Array(typed),
            _ => throw new ArgumentException($"Invalid or unsupported type ({inputValue?.GetType()})", nameof(inputValue)),
        };
    }
    public static JsonDataValue Object() => new JsonDataObject();
    public static JsonDataValue Array() => new JsonDataArray();
    public static JsonDataValue Array<T>(IEnumerable<T> values) => new(new JsonDataArray(values?.OfType<object?>().Select(Create)));

    //public static Raw Array<T>(params T?[] values) => CreateArray(new RawArray(values?.OfType<object?>().Select(Create)));
    public static JsonDataValue Object<T>(IReadOnlyDictionary<string, T>? values)
    {
        JsonDataObject source = new(values?.Count ?? 0);

        if (values != null)
        {
            foreach ((string key, T value) in values)
            {
                source[key] = Create(value);
            }
        }

        return new(source);
    }
    public string ToJsonFragment()
    {
        return _value switch
        {
            null => "null",
            bool b => b ? "true" : "false",
            double n => n.ToString(CultureInfo.InvariantCulture),
            string s => $"\"{s}\"",
            JsonDataArray a => a.ToJsonFragment(),
            JsonDataObject o => o.ToJsonFragment(),
            _ => throw new InvalidOperationException("Type is invalid"),
        };
    }


    public static implicit operator JsonDataValue(Guid value) => new(value.ToString());
    public static implicit operator JsonDataValue(DomainId value) => new(value);
    public static implicit operator JsonDataValue(DateTime value) => new(value.ToString());
    public static implicit operator JsonDataValue(bool value) => new(value);
    public static implicit operator JsonDataValue(double value) => new(value);
    public static implicit operator JsonDataValue(float value) => new(value);
    public static implicit operator JsonDataValue(int value) => new(value);
    public static implicit operator JsonDataValue(long value) => new(value);
    public static implicit operator JsonDataValue(string? value) => new(value);
    public static implicit operator JsonDataValue(JsonDataArray? value) => new(value);
    public static implicit operator JsonDataValue(JsonDataObject? value) => new(value);

    private static void ThrowInvalidType()
    {
        throw new InvalidCastException("Invalid type");
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is JsonDataValue typed && Equals(typed);
    }

    public bool Equals(JsonDataValue other)
    {
        return Equals(other._value, _value);
    }

    public override int GetHashCode()
    {
        return _value?.GetHashCode() * 31 ?? 0;
    }

    public void FromSource(object? source)
    {
        throw new NotImplementedException();
    }

    public object? ToTarget()
    {
        return Type switch
        {
            JsonDataValueType.Object => AsObject().ToSource(),
            JsonDataValueType.Array => AsArray().ToSource(),
            JsonDataValueType.Number => AsNumber(),
            JsonDataValueType.String => AsString(),
            JsonDataValueType.Boolean => AsBoolean(),
            _ => null,
        };
    }

    public static bool operator ==(JsonDataValue left, JsonDataValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JsonDataValue left, JsonDataValue right)
    {
        return !(left == right);
    }
}
