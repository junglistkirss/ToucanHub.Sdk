using System.Text.Json;
using System.Text.Json.Serialization;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Converters;

public sealed class TenantConverter : JsonConverter<Tenant>
{
    public override Tenant Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Tenant.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, Tenant value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}
