using System.Text.Json;
using System.Text.Json.Serialization;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Converters;

public sealed class TagConverter : JsonConverter<Tag>
{
    public override Tag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (Tag.TryParse(reader.GetString(), out Tag slug))
            return slug;
        return Tag.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Tag value, JsonSerializerOptions options)
    {
        if (value != Tag.Empty)
            writer.WriteStringValue(value.Name);
        else
            writer.WriteNullValue();
    }
    public override Tag ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (Tag.TryParse(reader.GetString(), out Tag slug))
            return slug;
        throw new NotSupportedException("PropertyName must be a not empty Tag");
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] Tag value, JsonSerializerOptions options)
    {
        if (value != Tag.Empty)
            writer.WritePropertyName(value.Name);
        else
            throw new NotSupportedException("PropertyName must be a not empty Tag");
    }
}