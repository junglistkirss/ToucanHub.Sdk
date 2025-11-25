using System.Text.Json;
using System.Text.Json.Serialization;
using ToucanHub.Sdk.Contracts.Names;

namespace ToucanHub.Sdk.Contracts.Converters;

public sealed class ActorReferenceConverter : JsonConverter<ActorReference>
{
    public override ActorReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => ActorReference.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, ActorReference value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}
