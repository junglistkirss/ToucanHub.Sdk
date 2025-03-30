using System.Collections;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
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
        CommonJson.ChangeJsonSerializerOptionsConfiguration((opts) =>
        {
            CommonJson.SdkContractsSerializerOptions(opts);
            opts.TypeInfoResolver = opts.TypeInfoResolver!
                .WithAddedModifier(TestResolver);
        });
    }

    private sealed record TestEvent1(Slug Key) : EventMessage;
    private sealed record TestEvent2(Slug OtherKey) : EventMessage;

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$my-type")]
    [JsonDerivedType(typeof(ImplementationEvent1), "implem1")]
    [JsonDerivedType(typeof(ImplementationEvent2), "implem2")]
    private abstract record AbstractEvent() : EventMessage;
    private sealed record ImplementationEvent1() : AbstractEvent;
    private sealed record ImplementationEvent2() : AbstractEvent;
    private sealed record RawEvent(byte[] Values) : AbstractEvent;

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
    public void Binary__ImplicitString()
    {
        string inline = CommonJson.Stringify(new RawEvent([0xCC]));
        RawEvent deserialized = CommonJson.FastRead<RawEvent>(inline);
        Assert.Single(deserialized.Values);
        Assert.Equal<byte>(0xCC, deserialized.Values[0]);
    }

    [Fact]
    public void Abstract__InlineCompare()
    {
        string inline = CommonJson.Stringify<AbstractEvent>(new ImplementationEvent1());
        AbstractEvent deserialized = CommonJson.FastRead<AbstractEvent>(inline);
        Assert.True(deserialized is ImplementationEvent1);
    }


    [Fact]
    public void Abstract__ByteCompare()
    {
        byte[] bytes = CommonJson.FastWrite<AbstractEvent>(new ImplementationEvent1());
        AbstractEvent deserialized = CommonJson.FastRead<AbstractEvent>(bytes);
        Assert.True(deserialized is ImplementationEvent1);
    }

    [Fact]
    public void ObjectSerialization__InlineCompare()
    {
        JsonDataObject dat = new(new Dictionary<string, JsonDataValue> { { "prop1", "string_value" } });
        string inline = CommonJson.Stringify(dat);
        JsonDataObject deserialized = CommonJson.FastRead<JsonDataObject>(inline);
        Assert.Equal(dat, deserialized);
    }

    [Fact]
    public void ObjectSerialization__Compare()
    {
        JsonDataObject dat = new(new Dictionary<string, JsonDataValue> { { "prop1", "string_value" } });
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
        JsonDataArray dat = new([(byte)1, (sbyte)1, (ushort)1, (short)1, (int)1, (uint)1, (long)1, (ulong)1, (float)1.0, (double)1.0, (decimal)1.0, "1"]);

        byte[] serialized = CommonJson.FastWrite(dat);
        JsonDataArray deserialized = CommonJson.FastRead<JsonDataArray>(serialized);
        Assert.Equal(dat.Count, deserialized.Count);

        for (int i = 0; i < dat.Count; i++)
        {
            Assert.True(dat[i].IsByte(out byte byteExpected));
            Assert.True(deserialized[i].IsByte(out byte byteActual));
            Assert.Equal(byteExpected, byteActual);

            Assert.True(dat[i].IsSByte(out sbyte sbyteExpected));
            Assert.True(deserialized[i].IsSByte(out sbyte sbyteActual));
            Assert.Equal(sbyteExpected, sbyteActual);

            Assert.True(dat[i].IsInt(out int intExpected));
            Assert.True(deserialized[i].IsInt(out int intActual));
            Assert.Equal(intExpected, intActual);

            Assert.True(dat[i].IsUInt(out uint uintExpected));
            Assert.True(deserialized[i].IsUInt(out uint uintActual));
            Assert.Equal(uintExpected, uintActual);

            Assert.True(dat[i].IsShort(out short shortExpected));
            Assert.True(deserialized[i].IsShort(out short shortActual));
            Assert.Equal(shortExpected, shortActual);

            Assert.True(dat[i].IsUShort(out ushort ushortExpected));
            Assert.True(deserialized[i].IsUShort(out ushort ushortActual));
            Assert.Equal(ushortExpected, ushortActual);

            Assert.True(dat[i].IsLong(out long longExpected));
            Assert.True(deserialized[i].IsLong(out long longActual));
            Assert.Equal(longExpected, longActual);

            Assert.True(dat[i].IsULong(out ulong ulongExpected));
            Assert.True(deserialized[i].IsULong(out ulong ulongActual));
            Assert.Equal(ulongExpected, ulongActual);

            Assert.True(dat[i].IsDecimal(out decimal decimalExpected));
            Assert.True(deserialized[i].IsDecimal(out decimal decimalActual));
            Assert.Equal(decimalExpected, decimalActual);

            Assert.True(dat[i].IsFloat(out float floatExpected));
            Assert.True(deserialized[i].IsFloat(out float floatActual));
            Assert.Equal(floatExpected, floatActual);

            Assert.True(dat[i].IsDouble(out double doubleExpected));
            Assert.True(deserialized[i].IsDouble(out double doubleActual));
            Assert.Equal(doubleExpected, doubleActual);
        }
    }

    [Fact]
    public void RawArraySerialization__CompareFloatingPointNumerics()
    {
        JsonDataArray dat = new([(byte)1, (sbyte)1, (ushort)1, (short)1, (int)1, (uint)1, (long)1, (ulong)1, (float)1.0, (double)1.0, (decimal)1.0, "1"]);

        byte[] serialized = CommonJson.FastWrite(dat);
        JsonDataArray deserialized = CommonJson.FastRead<JsonDataArray>(serialized);
        Assert.Equal(dat.Count, deserialized.Count);

        for (int i = 0; i < dat.Count; i++)
        {
            Assert.True(dat[i].IsDecimal(out decimal decimalExpected));
            Assert.True(deserialized[i].IsDecimal(out decimal decimalActual));
            Assert.Equal(decimalExpected, decimalActual);

            Assert.True(dat[i].IsFloat(out float floatExpected));
            Assert.True(deserialized[i].IsFloat(out float floatActual));
            Assert.Equal(floatExpected, floatActual);

            Assert.True(dat[i].IsDouble(out double doubleExpected));
            Assert.True(deserialized[i].IsDouble(out double doubleActual));
            Assert.Equal(doubleExpected, doubleActual);
        }
    }

    [Fact]
    public void DictionarySerialization__Compare()
    {
        JsonDataValue dat = JsonDataValue.Create((IDictionary)(new Dictionary<string, object?>() { { "key", 123 } }));

        byte[] serialized = CommonJson.FastWrite(dat);
        JsonDataObject deserialized = CommonJson.FastRead<JsonDataObject>(serialized);
        Assert.Equal(123, dat.AsObject()["key"].AsInt());
        Assert.Equal(123, deserialized["key"].AsInt());
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
