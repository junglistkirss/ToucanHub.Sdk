using System.Text.Json;
using System.Text.Json.Serialization;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Converters;

public sealed class DomainIdConverter : JsonConverter<DomainId>
{
    public override DomainId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (DomainId.TryParse(reader.GetString(), out DomainId slug))
            return slug;
        return DomainId.Empty;
    }

    public override void Write(Utf8JsonWriter writer, DomainId value, JsonSerializerOptions options)
    {
        if (value != DomainId.Empty)
            writer.WriteStringValue(value.Id);
        else
            writer.WriteNullValue();
    }
    public override DomainId ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (DomainId.TryParse(reader.GetString(), out DomainId slug))
            return slug;
        throw new NotSupportedException("PropertyName must be a not empty DomainId");
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] DomainId value, JsonSerializerOptions options)
    {
        if (value != DomainId.Empty)
            writer.WritePropertyName(value.Id);
        else
            throw new NotSupportedException("PropertyName must be a not empty DomainId");
    }
}
