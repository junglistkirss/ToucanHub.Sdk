using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ToucanHub.Sdk.Contracts.Converters;

namespace ToucanHub.Sdk.Contracts.JsonData;


public static class JsonDataSerializer
{
    public static ValueTask FastWriteAsync<T>(Stream stream, T message, CancellationToken cancellationToken = default) => FastWriteAsync(stream, message, typeof(T), cancellationToken);
    public static async ValueTask FastWriteAsync(Stream stream, object? message, Type type, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new NullReferenceException("Object to serialize is empty");

        if (_serializerOptionsInstance.TryGetTypeInfo(type, out JsonTypeInfo? typeInfo))
        {
            await JsonSerializer.SerializeAsync(stream, message, typeInfo, cancellationToken);
        }
        else
        {
            await JsonSerializer.SerializeAsync(stream, message, type, _serializerOptionsInstance, cancellationToken);
        }
    }

    public static ValueTask<T?> FastReadAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        => FastReadAsync<T>(stream, typeof(T), cancellationToken);

    public static async ValueTask<T?> FastReadAsync<T>(Stream stream, Type type, CancellationToken cancellationToken = default)
    {
        // if (type != typeof(T) && !type.IsAssignableTo(typeof(T)))
        //     throw new InvalidCastException("Unable to read data, types are incompatibles");

        if (_serializerOptionsInstance.TryGetTypeInfo(type, out JsonTypeInfo? typeInfo) && typeInfo is JsonTypeInfo<T> typed)
        {
            return await JsonSerializer.DeserializeAsync(stream, typed, cancellationToken);
        }
        object? json = await JsonSerializer.DeserializeAsync(stream, type, _serializerOptionsInstance, cancellationToken);
        if (json is null)
            return default;
        return (T)json;
    }

    public static T? FastRead<T>(string? inline) => FastRead<T>(inline, typeof(T));
    public static T? FastRead<T>(string? inline, Type type)
    {
        if (string.IsNullOrWhiteSpace(inline))
            return default;
        return FastRead<T>(Encoding.UTF8.GetBytes(inline), type);
    }

    public static string? Stringify<T>(T message) => Stringify(message, typeof(T));
    public static string? Stringify(object? message, Type type)
    {
        if (message is null)
            return null;
        byte[] dat = FastWrite(message, type);
        return Encoding.UTF8.GetString(dat);
    }

    public static T? FastRead<T>(byte[] bytes)
    {
        if (bytes is null)
            return default;
        return FastRead<T>(bytes, typeof(T));
    }

    public static T? FastRead<T>(byte[] bytes, Type type)
    {
        if (bytes is null)
            return default;
        return FastRead<T>(bytes.AsSpan(), type);
    }

    public static T? FastRead<T>(Span<byte> bytes, Type type)
    {
        if (bytes.IsEmpty)
            return default;

        Utf8JsonReader reader = new(bytes, new JsonReaderOptions
        {
            AllowTrailingCommas = true,
        });
        if (_serializerOptionsInstance.TryGetTypeInfo(type, out JsonTypeInfo? typeInfo) && typeInfo is JsonTypeInfo<T> typed)
            return JsonSerializer.Deserialize(ref reader, typed);
        object? json = JsonSerializer.Deserialize(ref reader, type, _serializerOptionsInstance);
        if (json is null)
            return default;
        return (T)json;
    }
    public static byte[] FastWrite<T>(T message) => FastWrite(message, typeof(T));
    public static byte[] FastWrite(object? message, Type type)
    {
        if (message is null)
            return [];

        if (!type.IsAssignableFrom(message.GetType()))
            throw new InvalidCastException("Unable to read data, types are incompatibles");


        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream, new JsonWriterOptions
        {
            Indented = false,
            SkipValidation = false,
        });
        JsonSerializer.Serialize(writer, message, type, _serializerOptionsInstance);
        writer.Flush();
        return stream.ToArray();
    }

    public static void SdkContractsSerializerOptions(JsonSerializerOptions options)
    {
        // not resolvers added here, options.TypeInfoResolver must be not null later !!
        options.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();

        options.Converters.Add(new DomainIdConverter());
        options.Converters.Add(new SlugConverter());
        options.Converters.Add(new TagConverter());
        options.Converters.Add(new ActorReferenceConverter());
        options.Converters.Add(new TenantConverter());
        options.Converters.Add(new ColorConverter());
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new JsonValueConverter());
        options.Converters.Add(new JsonObjectConverter());
        options.Converters.Add(new JsonArrayConverter());
    }
    private static readonly JsonSerializerOptions _baseJsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };
    public static Action<JsonSerializerOptions> JsonSerializerOptionsConfiguration { get; private set; } = SdkContractsSerializerOptions;

    public static void ChangeJsonSerializerOptionsConfiguration(Action<JsonSerializerOptions> action)
    {
        JsonSerializerOptionsConfiguration = action;
        _serializerOptionsInstance = GetJsonSerializerOptions();
    }


    private static JsonSerializerOptions _serializerOptionsInstance = GetJsonSerializerOptions();
    public static JsonSerializerOptions SerializerOptionsInstance => _serializerOptionsInstance;


    /// <summary>
    /// Create a new insatnce of <see cref="JsonSerializerOptions"/> configured with static method <see cref="JsonSerializerOptionsConfiguration"/>
    /// </summary>
    /// <param name="baseOptions"></param>
    /// <returns></returns>
    public static JsonSerializerOptions GetJsonSerializerOptions(JsonSerializerOptions? baseOptions = null)
    {
        JsonSerializerOptions options = new(baseOptions ?? _baseJsonSerializerOptions);
        JsonSerializerOptionsConfiguration(options);
        return options;
    }
}
