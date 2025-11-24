using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ToucanHub.Sdk.Contracts.Converters;

namespace ToucanHub.Sdk.Contracts.JsonData;


public static class JsonDataSerializer
{
    public static T FastRead<T>(string inline) => FastRead<T>(inline, typeof(T));
    public static T FastRead<T>(string inline, Type type) => FastRead<T>(Encoding.UTF8.GetBytes(inline), type);

    public static string Stringify<T>(T message) => Stringify(message, typeof(T));
    public static string Stringify(object? message, Type type)
    {
        byte[] dat = FastWrite(message, type);
        return Encoding.UTF8.GetString(dat);
    }

    public static T FastRead<T>(byte[] bytes) => FastRead<T>(bytes, typeof(T));
    public static T FastRead<T>(byte[] bytes, Type type) => FastRead<T>(bytes.AsSpan(), type);
    public static T FastRead<T>(Span<byte> bytes, Type type)
    {
        if (type != typeof(T) && !type.IsAssignableTo(typeof(T)))
            throw new InvalidCastException("Unable to read data, types are incompatibles");

        if (bytes.Length == 0)
            return default!;

        Utf8JsonReader reader = new(bytes, new JsonReaderOptions
        {
            AllowTrailingCommas = true,
        });
        if (_serializerOptionsInstance.TryGetTypeInfo(type, out JsonTypeInfo? typeInfo) && typeInfo is JsonTypeInfo<T> typed)
            return JsonSerializer.Deserialize(ref reader, typed)!;
        object? json = JsonSerializer.Deserialize(ref reader, type, _serializerOptionsInstance);
        return (T)json!;
    }
    public static byte[] FastWrite<T>(T message) => FastWrite(message, typeof(T));
    public static byte[] FastWrite(object? message, Type type)
    {
        if (message == null)
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
        options.TypeInfoResolver = options.TypeInfoResolver ?? new DefaultJsonTypeInfoResolver();

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
