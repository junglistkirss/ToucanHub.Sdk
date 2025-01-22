using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using Toucan.Sdk.Contracts.JsonData;
using Toucan.Sdk.Contracts.Messages;
using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Registry;

namespace Toucan.Sdk.Contracts.Tests;

public class SeralizeTests
{
    private static void TestResolver(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Kind == JsonTypeInfoKind.None)
            return;

        Type type = typeof(EventMessage);
        if (type == jsonTypeInfo.Type)
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$test-type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
                DerivedTypes =
                    {
                        new JsonDerivedType(typeof(TestEvent1), "TestEvent1"),
                        new JsonDerivedType(typeof(TestEvent2), "TestEvent2")
                    }
            };
        }

    }

    public SeralizeTests()
    {
        CommonJson.ChangeJsonSerializerOptionsConfiguration( (opts) =>
        {
            CommonJson.SdkContractsSerializerOptions(opts);
            opts.TypeInfoResolver = opts.TypeInfoResolver!
                .WithAddedModifier(TestResolver);
        });
    }

    private sealed record TestEvent1(Slug Key) : EventMessage;
    private sealed record TestEvent2(Slug OtherKey) : EventMessage;
    
    [Fact]
    public void BatchSerialisation()
    {
        TypeNameRegistry.Instance.Map<TestEvent1>("test event 1");
        TypeNameRegistry.Instance.Map<TestEvent2>("test event 2");

        EventMessage[] dat = [
                new TestEvent1("test1"),
                new TestEvent2("test2")
            ];
        string inline = CommonJson.Stringify(dat);
        EventMessage[] deserialized = CommonJson.FastRead<EventMessage[]>(inline);
        Assert.True(dat.SequenceEqual(deserialized));
    }



    [Fact]
    public void ObjectSerialization__InlineCompare()
    {
        JsonDataObject dat = new() { { "prop1", "string_value" } };
        string inline = CommonJson.Stringify(dat);
        JsonDataObject deserialized = CommonJson.FastRead<JsonDataObject>(inline);
        Assert.Equal(dat, deserialized);
    }

    [Fact]
    public void ObjectSerialization__Compare()
    {
        JsonDataObject dat = new() { { "prop1", "string_value" } };
        byte[] serialized = CommonJson.FastWrite(dat);
        JsonDataObject deserialized = CommonJson.FastRead<JsonDataObject>(serialized);

        Assert.Equal(dat, deserialized);
    }

    [Fact]
    public void ObjectDeserialization__Fails()
    {
        Assert.ThrowsAny<InvalidCastException>(() => CommonJson.FastRead<JsonDataObject>([0x05, 0x05], typeof(JsonDataArray)));
        Assert.ThrowsAny<InvalidCastException>(() => CommonJson.FastRead<JsonDataObject>([], typeof(JsonDataArray)));
    }

    [Fact]
    public void ObjectSerialization__Fails()
    {
        Assert.ThrowsAny<InvalidCastException>(() => CommonJson.FastWrite(12, typeof(JsonDataArray)));
        Assert.ThrowsAny<InvalidCastException>(() => CommonJson.FastWrite(new JsonDataObject(), typeof(JsonDataArray)));
    }

    [Fact]
    public void ObjectDeserialization__Default()
    {
        JsonDataObject result = CommonJson.FastRead<JsonDataObject>([], typeof(JsonDataObject));
        Assert.Null(result);
    }

    [Fact]
    public void ObjectSerialization__Default()
    {
        byte[] result = CommonJson.FastWrite(null!, typeof(JsonDataObject));
        Assert.Empty(result);
    }

    [Fact]
    public void RawArraySerialization__Compare()
    {
        JsonDataArray dat = [1, "string_value"];

        byte[] serialized = CommonJson.FastWrite(dat);
        JsonDataArray deserialized = CommonJson.FastRead<JsonDataArray>(serialized);

        for (int i = 0; i < dat.Count; i++)
            Assert.Equal(dat[i], deserialized[i]);
    }

    [Fact]
    public void IReadOnlyDictionarySerialization__Compare()
    {
        IReadOnlyDictionary<Slug, JsonDataObject> dat = new Dictionary<Slug, JsonDataObject>() { { "key", [] } };

        byte[] serialized = CommonJson.FastWrite(dat);
        IReadOnlyDictionary<Slug, JsonDataObject> deserialized = CommonJson.FastRead<IReadOnlyDictionary<Slug, JsonDataObject>>(serialized);
        Assert.True(dat.SequenceEqual(deserialized));
    }
}
