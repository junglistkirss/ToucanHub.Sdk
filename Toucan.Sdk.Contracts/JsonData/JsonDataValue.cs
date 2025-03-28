using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Toucan.Sdk.Contracts.Extensions;
using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.JsonData;

public readonly struct JsonDataValue : IEquatable<JsonDataValue>, ITarget<object?>
{

    public static readonly JsonDataValue Null = new(null, JsonDataValueType.Null);
    public static readonly JsonDataValue True = new(true);
    public static readonly JsonDataValue False = new(false);
    //public static readonly JsonDataValue Zero = new(0.0d);

    private readonly object? _value;
    private readonly JsonDataValueType _type;

    public JsonDataValueType Type => _type;

    public object? RawValue => _value;

    private JsonDataValue(object? value, JsonDataValueType type)
    {
        _value = value;
        _type = type;
    }
    public JsonDataValue(byte[] value) : this(value, JsonDataValueType.String) { }
    public JsonDataValue(byte value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(sbyte value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(short value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(ushort value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(int value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(uint value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(long value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(ulong value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(double value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(decimal value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(float value) : this(value, JsonDataValueType.Number) { }
    public JsonDataValue(DateTime value) : this(value, JsonDataValueType.String) { }
    public JsonDataValue(DateTimeOffset value) : this(value, JsonDataValueType.String) { }
    public JsonDataValue(TimeSpan value) : this(value, JsonDataValueType.String) { }
    public JsonDataValue(DateOnly value) : this(value, JsonDataValueType.String) { }
    public JsonDataValue(TimeOnly value) : this(value, JsonDataValueType.String) { }
    public JsonDataValue(bool value) : this(value, JsonDataValueType.Boolean) { }
    public JsonDataValue(string? value) : this(value, JsonDataValueType.String) { }
    public JsonDataValue(JsonDataArray value) : this(value, JsonDataValueType.Array) { }
    public JsonDataValue(JsonDataObject value) : this(value, JsonDataValueType.Object) { }
    public JsonDataValue(Guid value) : this(value, JsonDataValueType.String) { }
    public JsonDataValue(DomainId value) : this(value, JsonDataValueType.String) { }
    public JsonDataValue(Slug value) : this(value, JsonDataValueType.String) { }


    public bool IsNull()
    {
        return Type == JsonDataValueType.Null;
    }

    public bool IsBoolean([NotNullWhen(true)] out bool result)
    {
        result = default;
        if (_value is bool typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Boolean)
        {
            result = Convert.ToBoolean(_value);
            return true;
        }
        else if (bool.TryParse(_value?.ToString(), out bool value))
        {
            result = value;
            return true;
        }
        return false;
    }

    public bool AsBoolean()
    {
        if (IsBoolean(out bool value))
            return value;
        throw new InvalidCastException();
    }

    //public bool IsNumber([NotNullWhen(true)] out object? result)
    //{
    //    result = null;

    //    if (Type == JsonDataValueType.Number)
    //    {
    //        result = AsNumber();
    //        return true;
    //    }
    //    return false;
    //}
    //public object AsNumber()
    //{
    //    return _value!;
    //}

    public bool IsByte([NotNullWhen(true)] out byte result)
    {
        result = default;
        if (_value is byte b)
        {
            result = b;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToByte(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && byte.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out byte value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public byte AsByte()
    {
        if (IsByte(out byte value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsSByte([NotNullWhen(true)] out sbyte result)
    {
        result = default;
        if (_value is sbyte typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToSByte(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && sbyte.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out sbyte value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public sbyte AsSByte()
    {
        if (IsSByte(out sbyte value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsShort([NotNullWhen(true)] out short result)
    {
        result = default;
        if (_value is short typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToInt16(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && short.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out short value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public short AsShort()
    {
        if (IsShort(out short value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsUShort([NotNullWhen(true)] out ushort result)
    {
        result = default;
        if (_value is ushort typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToUInt16(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && ushort.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out ushort value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public ushort AsUShort()
    {
        if (IsUShort(out ushort value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsInt([NotNullWhen(true)] out int result)
    {
        result = default;
        if (_value is int typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToInt32(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && int.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out int value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public int AsInt()
    {
        if (IsInt(out int value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsUInt([NotNullWhen(true)] out uint result)
    {
        result = default;
        if (_value is uint typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToUInt32(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && uint.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out uint value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public uint AsUInt()
    {
        if (IsUInt(out uint value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsLong([NotNullWhen(true)] out long result)
    {
        result = default;
        if (_value is long typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToInt64(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && long.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out long value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public long AsLong()
    {
        if (IsLong(out long value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsFloat([NotNullWhen(true)] out float result)
    {
        result = default;
        if (_value is float typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToSingle(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && float.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out float value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public float AsFloat()
    {
        if (IsFloat(out float value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsDouble([NotNullWhen(true)] out double result)
    {
        result = default;
        if (_value is double typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToDouble(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && double.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out double value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public double AsDouble()
    {
        if (IsDouble(out double value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsDecimal([NotNullWhen(true)] out decimal result)
    {
        result = default;
        if (_value is decimal typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToDecimal(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && decimal.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out decimal value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public decimal AsDecimal()
    {
        if (IsDecimal(out decimal value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsULong([NotNullWhen(true)] out ulong result)
    {
        result = default;
        if (_value is ulong typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Number)
        {
            result = Convert.ToUInt64(_value);
            return true;
        }
        if (Type == JsonDataValueType.String && ulong.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out ulong value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public ulong AsULong()
    {
        if (IsULong(out ulong value))
            return value;
        throw new InvalidCastException();
    }
    public bool IsString([NotNullWhen(true)] out string? result)
    {
        result = null;
        if (_value is string typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.String)
        {
            result = _value?.ToString()!;
            return true;
        }
        return false;
    }
    public string? AsString()
    {
        if (IsString(out string? str))
            return str;
        throw new InvalidCastException();
    }

    public bool IsBinary([NotNullWhen(true)] out byte[]? result)
    {
        result = null;
        if (_value is byte[] typed)
        {
            result = typed;
            return true;
        }
        if (IsString(out string? str))
        {
            result = Convert.FromBase64String(str);
            return true;
        }
        return false;
    }
    public byte[]? AsBinary()
    {
        if (IsBinary(out byte[]? str))
            return str;
        throw new InvalidCastException();
    }
    public bool IsGuid([NotNullWhen(true)] out Guid result)
    {
        result = Guid.Empty;

        if (_value is Guid gid)
        {
            result = gid;
            return true;
        }
        else if (Guid.TryParse(_value?.ToString(), out Guid id))
        {
            result = id;
            return true;
        }
        return false;
    }


    public bool IsDomainId([NotNullWhen(true)] out DomainId result)
    {
        result = DomainId.Empty;

        if (_value is DomainId did)
        {
            result = did;
            return true;
        }
        else if (DomainId.TryParse(_value?.ToString(), out DomainId id))
        {
            result = id;
            return true;
        }
        return false;
    }

    public bool IsSlug([NotNullWhen(true)] out Slug result)
    {
        result = Slug.Empty;

        if (_value is Slug s)
        {
            result = s;
            return true;
        }
        else if (Slug.TryParse(_value?.ToString(), out Slug id))
        {
            result = id;
            return true;
        }
        return false;
    }

    public bool IsDateTime([NotNullWhen(true)] out DateTime result)
    {
        result = default!;

        if (_value is DateTime typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.String && DateTime.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out DateTime value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public DateTime AsDateTime()
    {
        if (IsDateTime(out DateTime res))
            return res;
        throw new InvalidCastException();
    }
    public bool IsDateTimeOffset([NotNullWhen(true)] out DateTimeOffset result)
    {
        result = default!;

        if (_value is DateTimeOffset typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.String && DateTimeOffset.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out DateTimeOffset value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public DateTimeOffset AsDateTimeOffset()
    {
        if (IsDateTimeOffset(out DateTimeOffset res))
            return res;
        throw new InvalidCastException();
    }
    public bool IsTimeSpan([NotNullWhen(true)] out TimeSpan result)
    {
        result = default!;

        if (_value is TimeSpan typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.String && TimeSpan.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out TimeSpan value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public TimeSpan AsTimeSpan()
    {
        if (IsTimeSpan(out TimeSpan res))
            return res;
        throw new InvalidCastException();
    }
    public bool IsDateOnly([NotNullWhen(true)] out DateOnly result)
    {
        result = default!;

        if (_value is DateOnly typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.String && DateOnly.TryParse(_value?.ToString(), CultureInfo.InvariantCulture, out DateOnly value))
        {
            result = value;
            return true;
        }
        return false;
    }
    public DateOnly AsDateOnly()
    {
        if (IsDateOnly(out DateOnly res))
            return res;
        throw new InvalidCastException();
    }
    public bool IsTimeOnly([NotNullWhen(true)] out TimeOnly result)
    {
        result = default!;

        if (_value is TimeOnly typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.String && TimeOnly.TryParse(_value!.ToString(), CultureInfo.InvariantCulture, out TimeOnly value))
        {
            result = value;
        }
        return false;
    }
    public TimeOnly AsTimeOnly()
    {
        if (IsTimeOnly(out TimeOnly res))
            return res;
        throw new InvalidCastException();
    }
    public bool IsArray([NotNullWhen(true)] out JsonDataArray result)
    {
        result = default!;
        if(_value is JsonDataArray typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Array)
        {
            result = AsArray();
            return true;
        }
        return false;
    }
    public JsonDataArray AsArray()
    {
        if (IsArray(out JsonDataArray res))
            return res;
        throw new InvalidCastException();
    }

    public bool IsObject([NotNullWhen(true)] out JsonDataObject result)
    {
        result = default!;
        if (_value is JsonDataObject typed)
        {
            result = typed;
            return true;
        }
        if (Type == JsonDataValueType.Object)
        {
            result = AsObject();
            return true;
        }
        return false;
    }

    public JsonDataObject AsObject()
    {
        if (IsObject(out JsonDataObject res))
            return res;
        throw new InvalidCastException();
    }

    private static JsonDataValue Enumerate(IEnumerable elements)
    {
        List<JsonDataValue> values = [];
        foreach (object? item in elements)
        {
            values.Add(Create(item));
        }
        return new JsonDataArray(values);
    }

    private static JsonDataValue Objectize(IDictionary elements)
    {
        Dictionary<string, JsonDataValue> values = [];
        IDictionaryEnumerator enumerator = elements.GetEnumerator();
        while(enumerator.MoveNext()) 
        {
            string? key = enumerator.Key.ToString();
            if (string.IsNullOrEmpty(key))
                throw new InvalidDataException("Null key not allowed");
            values.Add(key, Create(enumerator.Value));
        }
        return new JsonDataObject(values);
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
                return new JsonDataValue(element.GetRawText(), JsonDataValueType.Unspecified);
            case JsonValueKind.String:
                return new(element.GetString(), JsonDataValueType.Unspecified);
            case JsonValueKind.Null:
                return Null;
            case JsonValueKind.Object:
                Dictionary<string, JsonDataValue> values = [];

                foreach (JsonProperty property in element.EnumerateObject())
                {
                    values.Add(property.Name, Parse(property.Value));
                }

                return new JsonDataObject(values);
            case JsonValueKind.Array:
                List<JsonDataValue> arr = [];

                foreach (JsonElement item in element.EnumerateArray())
                {
                    arr.Add(Parse(item));
                }

                return new JsonDataArray(arr);
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
            Slug typed => typed,
            DomainId typed => typed,
            DateTime typed => typed,
            DateTimeOffset typed => typed,
            DateOnly typed => typed,
            TimeOnly typed => typed,
            TimeSpan typed => typed,
            bool typed => typed,
            decimal typed => typed,
            float typed => typed,
            double typed => typed,
            byte typed => typed,
            sbyte typed => typed,
            short typed => typed,
            ushort typed => typed,
            int typed => typed,
            uint typed => typed,
            long typed => typed,
            ulong typed => typed,
            string typed => typed,
            byte[] typed => typed,

            JsonDataArray typed => typed,
            JsonDataObject typed => typed,

            JsonElement typed => Parse(typed),
            JsonDocument typed => Parse(typed.RootElement),

            //DomainId[] typed => Array(typed),
            //IEnumerable<JsonDataObject> typed => Array(typed),
            //IReadOnlyDictionary<string, object?> typed => Object(typed),
            //IEnumerable<KeyValuePair<string, object?>> typed => Array(typed),
            //object[] typed => Array(typed),
            _ => (inputValue is IDictionary dic) ? Objectize(dic) : ((inputValue is IEnumerable list) ? Enumerate(list) : throw new ArgumentException($"Invalid or unsupported type ({inputValue?.GetType()})", nameof(inputValue))),
        };
    }
    public static JsonDataValue EmptyObject() => new JsonDataObject();
    public static JsonDataValue EmptyArray() => new JsonDataArray();
    public static JsonDataValue Array<T>(IEnumerable<T> values) => new JsonDataArray(values?.OfType<object?>().Select(Create));

    //public static Raw Array<T>(params T?[] values) => CreateArray(new RawArray(values?.OfType<object?>().Select(Create)));
    public static JsonDataValue Object<T>(IReadOnlyDictionary<string, T>? values)
    {
        Dictionary<string, JsonDataValue> source = new(values?.Count ?? 0);

        if (values != null)
        {
            foreach ((string key, T value) in values)
            {
                source[key] = Create(value);
            }
        }

        return new JsonDataObject(source);
    }
    public string ToJsonFragment()
    {
        return _value switch
        {
            null => "null",
            bool b => b ? "true" : "false",
            float n => n.ToString(CultureInfo.InvariantCulture),
            decimal n => n.ToString(CultureInfo.InvariantCulture),
            double n => n.ToString(CultureInfo.InvariantCulture),
            ushort n => n.ToString(CultureInfo.InvariantCulture),
            short n => n.ToString(CultureInfo.InvariantCulture),
            uint n => n.ToString(CultureInfo.InvariantCulture),
            int n => n.ToString(CultureInfo.InvariantCulture),
            ulong n => n.ToString(CultureInfo.InvariantCulture),
            long n => n.ToString(CultureInfo.InvariantCulture),
            DateOnly n => n.ToString(CultureInfo.InvariantCulture),
            DateTime n => n.ToString(CultureInfo.InvariantCulture),
            DateTimeOffset n => n.ToString(CultureInfo.InvariantCulture),
            TimeOnly n => n.ToString(CultureInfo.InvariantCulture),
            TimeSpan n => n.ToString(),
            Guid n => n.ToString(),
            DomainId n => n.Id,
            Slug n => n.Value,
            string s => $"\"{s}\"",
            JsonDataArray a => a.ToJsonFragment(),
            JsonDataObject o => o.ToJsonFragment(),
            _ => throw new InvalidOperationException("Type is invalid"),
        };
    }
   

    public static implicit operator JsonDataValue(byte[] value) => new(value);

    public static implicit operator JsonDataValue(Guid value) => new(value);
    public static implicit operator JsonDataValue(Slug value) => new(value);
    public static implicit operator JsonDataValue(DomainId value) => new(value);

    public static implicit operator JsonDataValue(TimeSpan value) => new(value);
    public static implicit operator JsonDataValue(DateTime value) => new(value);
    public static implicit operator JsonDataValue(DateTimeOffset value) => new(value);
    public static implicit operator JsonDataValue(DateOnly value) => new(value);
    public static implicit operator JsonDataValue(TimeOnly value) => new(value);

    public static implicit operator JsonDataValue(bool value) => new(value);
    public static implicit operator JsonDataValue(byte value) => new(value);
    public static implicit operator JsonDataValue(sbyte value) => new(value);
    public static implicit operator JsonDataValue(short value) => new(value);
    public static implicit operator JsonDataValue(ushort value) => new(value);
    public static implicit operator JsonDataValue(int value) => new(value);
    public static implicit operator JsonDataValue(uint value) => new(value);
    public static implicit operator JsonDataValue(long value) => new(value);
    public static implicit operator JsonDataValue(ulong value) => new(value);
    public static implicit operator JsonDataValue(float value) => new(value);
    public static implicit operator JsonDataValue(decimal value) => new(value);
    public static implicit operator JsonDataValue(double value) => new(value);
    public static implicit operator JsonDataValue(string? value) => new(value);
    public static implicit operator JsonDataValue(JsonDataArray value) => new(value);
    public static implicit operator JsonDataValue(JsonDataObject value) => new(value);

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

    public object? ToTarget()
    {
        return Type switch
        {
            JsonDataValueType.Null => null,
            JsonDataValueType.Object => AsObject().ToSource(),
            JsonDataValueType.Array => AsArray().ToSource(),
            _ => _value,
        };
    }

    //public JsonDataValue Clone()
    //{
    //    return Type switch
    //    {
    //        JsonDataValueType.Null => Null,
    //        JsonDataValueType.Object => Create(AsObject().ToSource()),
    //        JsonDataValueType.Array => Create(AsArray().ToSource()),
    //        _ => Create(_value!),
    //    };
    //}

    public static bool operator ==(JsonDataValue left, JsonDataValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JsonDataValue left, JsonDataValue right)
    {
        return !(left == right);
    }
}
