using System.Text.Json;
using System.Text.Json.Serialization;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Converters;

public sealed class RefTokenConverter : JsonConverter<RefToken>
{
    public override RefToken Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => RefToken.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, RefToken value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}
