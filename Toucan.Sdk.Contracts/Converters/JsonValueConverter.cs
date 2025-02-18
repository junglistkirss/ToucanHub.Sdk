using System.Text.Json;
using System.Text.Json.Serialization;
using Toucan.Sdk.Contracts.JsonData;

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
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.StartObject => JsonSerializer.Deserialize<JsonDataObject>(ref reader, options),
            JsonTokenType.StartArray => JsonSerializer.Deserialize<JsonDataArray>(ref reader, options),
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
                writer.WriteStringValue(value.AsString());
                break;
            case JsonDataValueType.Number:
                writer.WriteNumberValue(value.AsNumber());
                break;
            default:
                throw new NotSupportedException();
        }

    }
}
