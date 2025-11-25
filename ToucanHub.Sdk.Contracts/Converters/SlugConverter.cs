using System.Text.Json;
using System.Text.Json.Serialization;
using ToucanHub.Sdk.Contracts.Names;

namespace ToucanHub.Sdk.Contracts.Converters;

public sealed class SlugConverter : JsonConverter<Slug>
{
    public override Slug Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (Slug.TryParse(reader.GetString(), out Slug slug))
            return slug;
        return Slug.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Slug value, JsonSerializerOptions options)
    {
        if (value != Slug.Empty)
            writer.WriteStringValue(value.Value);
        else
            writer.WriteNullValue();
    }
    public override Slug ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (Slug.TryParse(reader.GetString(), out Slug slug))
            return slug;
        throw new NotSupportedException("PropertyName must be a not empty Slug");
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] Slug value, JsonSerializerOptions options)
    {
        if (value != Slug.Empty)
            writer.WritePropertyName(value.Value);
        else
            throw new NotSupportedException("PropertyName must be a not empty Slug");
    }
}
