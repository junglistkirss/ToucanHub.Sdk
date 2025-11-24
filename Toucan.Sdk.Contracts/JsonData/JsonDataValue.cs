using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Toucan.Sdk.Contracts.Extensions;
using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.JsonData;

public readonly struct JsonDataValue : IEquatable<JsonDataValue>
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

    public bool IsNumber([NotNullWhen(true)] out object? result)
    {
        result = null;

        if (Type == JsonDataValueType.Number)
        {
            result = AsNumber();
            return true;
        }
        return false;
    }
    public object AsNumber()
    {
        return _value!;
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
        while (enumerator.MoveNext())
        {
            string? key = enumerator.Key.ToString();
            if (string.IsNullOrEmpty(key))
                throw new InvalidDataException("Null key not allowed");
            values.Add(key, Create(enumerator.Value));
        }
        return new JsonDataObject(values);
    }

    private static JsonDataValue ParseNode(JsonNode? element)
    {
        if (element is null)
            return Null;

        switch (element.GetValueKind())
        {
            case JsonValueKind.False:
                return False;
            case JsonValueKind.True:
                return True;
            case JsonValueKind.Number:
                return new JsonDataValue(element.AsValue(), JsonDataValueType.Number);
            case JsonValueKind.String:
                return new(element.AsValue(), JsonDataValueType.String);
            case JsonValueKind.Null:
                return Null;
            case JsonValueKind.Object:
                Dictionary<string, JsonDataValue> values = [];

                foreach (KeyValuePair<string, JsonNode?> property in element.AsObject())
                {
                    values.Add(property.Key, ParseNode(property.Value));
                }

                return new JsonDataObject(values);
            case JsonValueKind.Array:
                List<JsonDataValue> arr = [];

                foreach (JsonNode? item in element.AsArray())
                {
                    arr.Add(ParseNode(item!));
                }

                return new JsonDataArray(arr);
            default:
                throw new NotSupportedException();
        }
    }
    private static JsonDataValue ParseElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.False:
                return False;
            case JsonValueKind.True:
                return True;
            case JsonValueKind.Number:
                return new JsonDataValue(element.GetRawText(), JsonDataValueType.Number);
            case JsonValueKind.String:
                return new(element.GetString(), JsonDataValueType.String);
            case JsonValueKind.Null:
                return Null;
            case JsonValueKind.Object:
                Dictionary<string, JsonDataValue> values = [];

                foreach (JsonProperty property in element.EnumerateObject())
                {
                    values.Add(property.Name, ParseElement(property.Value));
                }

                return new JsonDataObject(values);
            case JsonValueKind.Array:
                List<JsonDataValue> arr = [];

                foreach (JsonElement item in element.EnumerateArray())
                {
                    arr.Add(ParseElement(item));
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

            JsonNode typed => ParseNode(typed),
            JsonElement typed => ParseElement(typed),
            JsonDocument typed => ParseElement(typed.RootElement),

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
    internal string ToJsonFragment()
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

    public static bool operator ==(JsonDataValue left, JsonDataValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JsonDataValue left, JsonDataValue right)
    {
        return !(left == right);
    }
}
