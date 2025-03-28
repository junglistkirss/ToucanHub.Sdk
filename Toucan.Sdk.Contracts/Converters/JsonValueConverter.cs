using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Toucan.Sdk.Contracts.JsonData;
using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.Converters;

public sealed class JsonObjectConverter : JsonConverter<JsonDataObject>
{
    public override JsonDataObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");

        Dictionary<string, JsonDataValue> values = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("JsonTokenType was not PropertyName");

            string? propertyName = reader.GetString();
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new JsonException("Failed to get property name");

            reader.Read();

            JsonDataValue value = JsonSerializer.Deserialize<JsonDataValue>(ref reader, options);
            values.Add(propertyName, value);
        }
        return new JsonDataObject(values);
    }

    public override void Write(Utf8JsonWriter writer, JsonDataObject value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (IReadOnlyDictionary<string, JsonDataValue>)value, options);
    }
}

public sealed class JsonArrayConverter : JsonConverter<JsonDataArray>
{
    public override JsonDataArray? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only array are supported");

        List<JsonDataValue> list = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            JsonDataValue value = JsonSerializer.Deserialize<JsonDataValue>(ref reader, options);
            list.Add(value);
        }
        return new JsonDataArray(list);
    }

    public override void Write(Utf8JsonWriter writer, JsonDataArray value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (IReadOnlyList<JsonDataValue>)value, options);
    }
}

public sealed class JsonValueConverter : JsonConverter<JsonDataValue>
{
    public override JsonDataValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Null => JsonDataValue.Null,
            JsonTokenType.Number =>
                reader.TryGetDouble(out double dbl) ? dbl
                : reader.TryGetInt64(out long l) ? l
                : reader.TryGetDecimal(out decimal dec) ? dec
                : throw new InvalidDataException(),
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.StartObject => JsonSerializer.Deserialize<JsonDataObject>(ref reader, options) ?? JsonDataValue.Null,
            JsonTokenType.StartArray => JsonSerializer.Deserialize<JsonDataArray>(ref reader, options) ?? JsonDataValue.Null,
            _ => throw new NotSupportedException("Unknown json token type"),
        };
    }

    public override void Write(Utf8JsonWriter writer, JsonDataValue value, JsonSerializerOptions options)
    {
        switch (value.Type)
        {
            case JsonDataValueType.Null:
                writer.WriteNullValue();
                break;
            case JsonDataValueType.Array:
                JsonSerializer.Serialize(writer, value.AsArray(), options);
                break;
            case JsonDataValueType.Object:
                JsonSerializer.Serialize(writer, value.AsObject(), options);
                break;
            case JsonDataValueType.Boolean:
                writer.WriteBooleanValue(value.AsBoolean());
                break;
            case JsonDataValueType.String:
                switch (value.RawValue)
                {
                    case byte n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case sbyte n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case float n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case decimal n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case double n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case ushort n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case short n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case uint n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case int n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case ulong n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case long n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case DateOnly n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case DateTime n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case DateTimeOffset n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case TimeOnly n:
                        writer.WriteStringValue(n.ToString(CultureInfo.InvariantCulture));
                        break;
                    case TimeSpan n:
                        writer.WriteStringValue(n.ToString());
                        break;
                    case Guid n:
                        writer.WriteStringValue(n.ToString());
                        break;
                    case DomainId n:
                        writer.WriteStringValue(n.Id);
                        break;
                    case Slug n:
                        writer.WriteStringValue(n.Value);
                        break;
                    case string s:
                        writer.WriteStringValue(s);
                        break;
                    case byte[] s:
                        writer.WriteBase64StringValue(s);
                        break;
                    default: throw new InvalidOperationException("Type is invalid");
                };
                break;
            case JsonDataValueType.Number:
                switch (value.RawValue)
                {
                    case byte n:
                        writer.WriteNumberValue(n);
                        break;
                    case sbyte n:
                        writer.WriteNumberValue(n);
                        break;
                    case float n:
                        writer.WriteNumberValue(n);
                        break;
                    case decimal n:
                        writer.WriteNumberValue(n);
                        break;
                    case double n:
                        writer.WriteNumberValue(n);
                        break;
                    case ushort n:
                        writer.WriteNumberValue(n);
                        break;
                    case short n:
                        writer.WriteNumberValue(n);
                        break;
                    case uint n:
                        writer.WriteNumberValue(n);
                        break;
                    case int n:
                        writer.WriteNumberValue(n);
                        break;
                    case ulong n:
                        writer.WriteNumberValue(n);
                        break;
                    case long n:
                        writer.WriteNumberValue(n);
                        break;
                    default:
                        throw new InvalidOperationException("Type is invalid");
                };
                break;
            default:
                throw new NotSupportedException();
        }

    }
}
