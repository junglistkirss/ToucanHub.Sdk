using System.Text.Json;
using System.Text.Json.Serialization;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Converters;

public sealed class ColorConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (Color.TryParse(reader.GetString(), out Color slug))
            return slug;
        return Color.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        if (value != Color.Empty)
            writer.WriteStringValue(value.HexCode);
        else
            writer.WriteNullValue();
    }
    public override Color ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (Color.TryParse(reader.GetString(), out Color slug))
            return slug;
        throw new NotSupportedException("PropertyName must be a not empty Color");
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] Color value, JsonSerializerOptions options)
    {
        if (value != Color.Empty)
            writer.WritePropertyName(value.HexCode);
        else
            throw new NotSupportedException("PropertyName must be a not empty Color");
    }
}
