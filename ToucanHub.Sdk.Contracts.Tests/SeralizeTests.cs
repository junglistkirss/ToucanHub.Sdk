using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ToucanHub.Sdk.Contracts.JsonData;
using ToucanHub.Sdk.Contracts.Names;

namespace ToucanHub.Sdk.Contracts.Tests;

internal interface IMessageTest { }
internal sealed record TestEvent1(Slug Key) : IMessageTest;
internal sealed record TestEvent2(Slug OtherKey) : IMessageTest;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$my-type")]
[JsonDerivedType(typeof(ImplementationEvent1), "i1")]
[JsonDerivedType(typeof(ImplementationEvent2), "i2")]
[JsonDerivedType(typeof(RawEvent), "raw")]
internal abstract record AbstractEvent();
internal sealed record ImplementationEvent1() : AbstractEvent;
internal sealed record ImplementationEvent2() : AbstractEvent;
internal sealed record RawEvent(byte[] Values) : AbstractEvent;
internal sealed record ExtraEvent(byte[] Values) : AbstractEvent;

public class SeralizeTests
{
    private static void TestResolver(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Kind == JsonTypeInfoKind.None)
            return;

        Type type = typeof(IMessageTest);
        if (type == jsonTypeInfo.Type)
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$test-type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
                DerivedTypes =
                    {
                        new JsonDerivedType(typeof(TestEvent1), nameof(TestEvent1)),
                        new JsonDerivedType(typeof(TestEvent2), nameof(TestEvent2))
                    }
            };
        }

    }

    public SeralizeTests()
    {
        JsonDataSerializer.ChangeJsonSerializerOptionsConfiguration((opts) =>
        {
            JsonDataSerializer.SdkContractsSerializerOptions(opts);
            opts.TypeInfoResolver = opts.TypeInfoResolver!
                .WithAddedModifier(TestResolver);
        });
    }


    [Fact]
    public void BatchSerialisation()
    {
        IMessageTest[] dat = [
                new TestEvent1("test1"),
                new TestEvent2("test2")
            ];
        string? inline = JsonDataSerializer.Stringify(dat);
        IMessageTest[]? deserialized = JsonDataSerializer.FastRead<IMessageTest[]>(inline);
        Assert.NotNull(deserialized);
        Assert.True(dat.SequenceEqual(deserialized));
    }

    [Fact]
    public async Task BatchSerialisationAsync()
    {
        IMessageTest[] dat = [
                new TestEvent1("test1"),
                new TestEvent2("test2")
            ];
        using MemoryStream stream = new();
        await JsonDataSerializer.FastWriteAsync(stream, dat, TestContext.Current.CancellationToken);
        await stream.FlushAsync(TestContext.Current.CancellationToken);
        stream.Position = 0;

        using MemoryStream streamCopy = new();
        await stream.CopyToAsync(streamCopy, TestContext.Current.CancellationToken);
        await streamCopy.FlushAsync(TestContext.Current.CancellationToken);
        streamCopy.Position = 0;

        IMessageTest[]? deserialized = await JsonDataSerializer.FastReadAsync<IMessageTest[]>(streamCopy, TestContext.Current.CancellationToken);
        Assert.NotNull(deserialized);
        Assert.True(dat.SequenceEqual(deserialized));
    }
    [Fact]
    public void Binary__ImplicitString()
    {
        string? inline = JsonDataSerializer.Stringify(new RawEvent([0xCC]));
        Assert.NotNull(inline);
        RawEvent? deserialized = JsonDataSerializer.FastRead<RawEvent>(inline);
        Assert.NotNull(deserialized);
        Assert.Single(deserialized.Values);
        Assert.Equal<byte>(0xCC, deserialized.Values[0]);
    }
    
    [Fact]
    public void Polymorĥic__MissingFails()
    {
        Assert.ThrowsAny<NotSupportedException>(()=> JsonDataSerializer.Stringify<AbstractEvent>(new ExtraEvent([0xCC])));
    }

    [Fact]
    public void Abstract__InlineCompare()
    {
        string? inline = JsonDataSerializer.Stringify<AbstractEvent>(new ImplementationEvent1());
        Assert.NotNull(inline);
        AbstractEvent? deserialized = JsonDataSerializer.FastRead<AbstractEvent>(inline);
        Assert.NotNull(deserialized);
        Assert.True(deserialized is ImplementationEvent1);
    }


    [Fact]
    public void Abstract__ByteCompare()
    {
        byte[] bytes = JsonDataSerializer.FastWrite<AbstractEvent>(new ImplementationEvent1());
        AbstractEvent? deserialized = JsonDataSerializer.FastRead<AbstractEvent>(bytes);
        Assert.NotNull(deserialized);
        Assert.True(deserialized is ImplementationEvent1);
    }

    [Fact]
    public void ObjectSerialization__InlineCompare()
    {
        JsonDataObject dat = new(new Dictionary<string, JsonDataValue> { { "prop1", "string_value" } });
        string? inline = JsonDataSerializer.Stringify(dat);
        Assert.NotNull(inline);
        JsonDataObject? deserialized = JsonDataSerializer.FastRead<JsonDataObject>(inline);
        Assert.NotNull(deserialized);
        Assert.Equal(dat, deserialized);
    }

    [Fact]
    public void ObjectSerialization__Compare()
    {
        JsonDataObject dat = new(new Dictionary<string, JsonDataValue> { { "prop1", "string_value" } });
        byte[] serialized = JsonDataSerializer.FastWrite(dat);
        JsonDataObject? deserialized = JsonDataSerializer.FastRead<JsonDataObject>(serialized);

        Assert.NotNull(deserialized);
        Assert.Equal(dat, deserialized);
    }

    [Fact]
    public void ObjectDeserialization__Fails()
    {
        Assert.ThrowsAny<JsonException>(() => JsonDataSerializer.FastRead<JsonDataObject>([0x05, 0x05], typeof(JsonDataArray)));
    }

    [Fact]
    public void ObjectSerialization__Fails()
    {
        Assert.ThrowsAny<InvalidCastException>(() => JsonDataSerializer.FastWrite(12, typeof(JsonDataArray)));
        Assert.ThrowsAny<InvalidCastException>(() => JsonDataSerializer.FastWrite(new JsonDataObject(), typeof(JsonDataArray)));
    }

    [Fact]
    public void ObjectDeserialization__Default()
    {
        JsonDataObject? result = JsonDataSerializer.FastRead<JsonDataObject>([], typeof(JsonDataObject));
        Assert.Null(result);
    }

    [Fact]
    public void ObjectSerialization__Default()
    {
        byte[] result = JsonDataSerializer.FastWrite(null!, typeof(JsonDataObject));
        Assert.Empty(result);
    }

    [Fact]
    public void RawArraySerialization__Compare()
    {
        JsonDataArray dat = new([(byte)1, (sbyte)1, (ushort)1, (short)1, 1, (uint)1, (long)1, (ulong)1, (float)1.0, (double)1.0, (decimal)1.0, "1"]);

        byte[] serialized = JsonDataSerializer.FastWrite(dat);
        JsonDataArray? deserialized = JsonDataSerializer.FastRead<JsonDataArray>(serialized);
        Assert.NotNull(deserialized);
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
        JsonDataArray dat = new([(byte)1, (sbyte)1, (ushort)1, (short)1, 1, (uint)1, (long)1, (ulong)1, (float)1.0, (double)1.0, (decimal)1.0, "1"]);

        byte[] serialized = JsonDataSerializer.FastWrite(dat);
        JsonDataArray? deserialized = JsonDataSerializer.FastRead<JsonDataArray>(serialized);
        Assert.NotNull(deserialized);
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
        JsonDataValue dat = JsonDataValue.Create(new Dictionary<string, object?>() { { "key", 123 } });

        byte[] serialized = JsonDataSerializer.FastWrite(dat);
        JsonDataObject? deserialized = JsonDataSerializer.FastRead<JsonDataObject>(serialized);
        Assert.NotNull(deserialized);
        Assert.Equal(123, dat.AsObject()["key"].AsInt());
        Assert.Equal(123, deserialized["key"].AsInt());
    }

    [Fact]
    public void IReadOnlyDictionarySerialization__Compare()
    {
        IReadOnlyDictionary<Slug, JsonDataObject> dat = new Dictionary<Slug, JsonDataObject>() { { "key", [] } };

        byte[] serialized = JsonDataSerializer.FastWrite(dat);
        IReadOnlyDictionary<Slug, JsonDataObject>? deserialized = JsonDataSerializer.FastRead<IReadOnlyDictionary<Slug, JsonDataObject>>(serialized);
        Assert.NotNull(deserialized);
        Assert.True(dat.SequenceEqual(deserialized));
    }
}
