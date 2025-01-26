using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Store.Extensions;
public static class PropertyBuilderExtensions
{
    private static DateTime WriteUtcDateTimeDataOrDie(DateTime fromCode, string name) => fromCode.Kind == DateTimeKind.Utc ? fromCode : throw new InvalidOperationException($"Column {name} only accepts UTC datetime values");
    private static DateTime ReadAsUtcDateTimeData(DateTime fromData) => fromData.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fromData, DateTimeKind.Utc) : fromData.ToUniversalTime();

    private static DateTime WriteTimestampDataOrDie(DateTimeOffset fromCode, string _) => fromCode.UtcDateTime;
    private static DateTimeOffset ReadAsTimestampData(DateTime fromData) => new(fromData);

    private static DateTime WriteLocalDateTimeDataOrDie(DateTime fromCode, string name) => fromCode.Kind == DateTimeKind.Local ? fromCode : throw new InvalidOperationException($"Column {name} only accepts Local datetime values");
    private static DateTime ReadAsLocalDateTimeData(DateTime fromData) => fromData.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(fromData, DateTimeKind.Local) : fromData.ToLocalTime();
    private static string WriteEnumDataOrDie<T>(T fromCode, string name)
            where T : struct => fromCode.ToString() ?? throw new InvalidOperationException($"Column {name} only accept enum values of {typeof(T).Name}");
    private static T ReadAsEnumData<T>(string fromData, T fallback)
            where T : struct => Enum.TryParse(fromData, true, out T value) ? value : fallback;

    //public static PropertyBuilder<byte[]> IsAutoVersionToken(this PropertyBuilder<byte[]> property) => property.IsConcurrencyToken().HasMaxLength();
    public static PropertyBuilder<string> IsEtagToken(this PropertyBuilder<string> property) => property.IsConcurrencyToken().HasMaxLength(256).IsRequired(true);



    public static PropertyBuilder<DomainId> IsDomainId(this PropertyBuilder<DomainId> property) => property.HasMaxLength(1000).IsRequired().IsUnicode(false).IsInlineStruct(DomainId.TryParse);

    public static PropertyBuilder<DomainId?> IsOptionnalDomainId(this PropertyBuilder<DomainId?> property) => property.HasMaxLength(1000).IsUnicode(false).IsOptionalInlineStruct(DomainId.TryParse);
    public static PropertyBuilder<Slug> IsSlug(this PropertyBuilder<Slug> property) => property.HasMaxLength(64).IsRequired().IsUnicode(false).IsInlineStruct(Slug.TryParse);
    public static PropertyBuilder<Slug?> IsOptionnalSlug(this PropertyBuilder<Slug?> property) => property.HasMaxLength(64).IsRequired(false).IsUnicode(false).IsOptionalInlineStruct(Slug.TryParse);

    public static PropertyBuilder<Color> IsColor(this PropertyBuilder<Color> property) => property.HasMaxLength(64).IsRequired().IsUnicode(false).IsInlineStruct(Color.TryParse);
    public static PropertyBuilder<Color?> IsOptionnalColor(this PropertyBuilder<Color?> property) => property.HasMaxLength(64).IsRequired(false).IsUnicode(false).IsOptionalInlineStruct(Color.TryParse);

    public static PropertyBuilder<Tag> IsTag(this PropertyBuilder<Tag> property) => property.HasMaxLength(100).IsRequired().IsUnicode(false).IsInlineStruct(Tag.TryParse);
    public static PropertyBuilder<string> IsName(this PropertyBuilder<string> property) => property.HasMaxLength(256).IsRequired().IsUnicode(false).IsRequired(true);
    public static PropertyBuilder<string?> IsOptionalName(this PropertyBuilder<string?> property) => property.HasMaxLength(256).IsUnicode(false).IsRequired(false);
    public static PropertyBuilder<string?> IsShortText(this PropertyBuilder<string?> property) => property.HasMaxLength(500).IsUnicode(true).IsRequired(false);
    public static PropertyBuilder<string> IsShortTextRequired(this PropertyBuilder<string> property) => property.HasMaxLength(500).IsUnicode(true).IsRequired(true);
    public static PropertyBuilder<string?> IsMediumText(this PropertyBuilder<string?> property) => property.HasMaxLength(1000).IsUnicode(true).IsRequired(false);

    public static PropertyBuilder<string> IsMediumTextRequired(this PropertyBuilder<string> property) => property.HasMaxLength(1000).IsUnicode(true).IsRequired(true);
    public static PropertyBuilder<string?> IsLargeText(this PropertyBuilder<string?> property) => property.HasMaxLength(4000).IsUnicode(true).IsRequired(false);

    public static PropertyBuilder<string> IsLargeTextReuired(this PropertyBuilder<string> property) => property.HasMaxLength(4000).IsUnicode(true).IsRequired(true);
    public static PropertyBuilder<T> IsEnum<T>(this PropertyBuilder<T> property, T fallback = default)
        where T : struct
    {
        return property.HasMaxLength(256).IsUnicode(false).HasConversion(
            fromCode => WriteEnumDataOrDie(fromCode, property.Metadata.Name),
            fromData => ReadAsEnumData(fromData, fallback)
        );
    }
    public static PropertyBuilder<T?> IsEnum<T>(this PropertyBuilder<T?> property, T fallback = default)
       where T : struct
    {
        return property.HasMaxLength(256).IsUnicode(false).HasConversion(
            fromCode => fromCode.HasValue ? WriteEnumDataOrDie(fromCode.Value, property.Metadata.Name) : null,
            fromData => !string.IsNullOrEmpty(fromData) ? ReadAsEnumData(fromData, fallback) : default(T?)
        );
    }

    public static PropertyBuilder<T> IsJsonValue<T>(this PropertyBuilder<T> property, JsonTypeInfo<T>? jsonTypeInfo = null)
    {
        Func<T, string> _s = (data) => JsonSerializer.Serialize(data);
        Func<string, T>? _d = (data) => (jsonTypeInfo != null ? JsonSerializer.Deserialize(data, jsonTypeInfo) : JsonSerializer.Deserialize<T>(data)) ?? default!;

        return property.HasMaxLength(4000).IsUnicode().HasConversion(
            fromCode => _s(fromCode),
            fromData => _d(fromData)
        );
    }

    public static PropertyBuilder<byte[]> IsBinary(this PropertyBuilder<byte[]> property, int size = 0)
    {
        PropertyBuilder<byte[]> prop = property.IsRequired();
        if (size > 0) prop.HasMaxLength(size);
        return prop;
    }

    public static PropertyBuilder<DateTime> IsUtcDateTime(this PropertyBuilder<DateTime> property)
    {
        return property.IsRequired().HasConversion(
            fromCode => WriteUtcDateTimeDataOrDie(fromCode, property.Metadata.Name),
            fromData => ReadAsUtcDateTimeData(fromData)
        );
    }
    public static PropertyBuilder<DateTime?> IsOptionalUtcDateTime(this PropertyBuilder<DateTime?> property)
    {
        return property.HasConversion(
            fromCode => fromCode != null ? WriteUtcDateTimeDataOrDie(fromCode.Value, property.Metadata.Name) : (DateTime?)null,
            fromData => fromData != null ? ReadAsUtcDateTimeData(fromData.Value) : null
        );
    }
    public static PropertyBuilder<DateTime> IsLocalDateTime(this PropertyBuilder<DateTime> property)
    {
        return property.IsRequired().HasConversion(
            fromCode => WriteLocalDateTimeDataOrDie(fromCode, property.Metadata.Name),
            fromData => ReadAsLocalDateTimeData(fromData)
        );
    }
    public static PropertyBuilder<DateTime?> IsOptionalLocalDateTime(this PropertyBuilder<DateTime?> property)
    {
        return property.HasConversion(
            fromCode => fromCode.HasValue ? WriteLocalDateTimeDataOrDie(fromCode.Value, property.Metadata.Name) : (DateTime?)null,
            fromData => fromData.HasValue ? ReadAsLocalDateTimeData(fromData.Value) : null
        );
    }

    public static PropertyBuilder<DateTimeOffset> IsTimestamp(this PropertyBuilder<DateTimeOffset> property)
    {
        return property.IsRequired().HasConversion(
            fromCode => WriteTimestampDataOrDie(fromCode, property.Metadata.Name),
            fromData => ReadAsTimestampData(fromData)
        );
    }
    public static PropertyBuilder<DateTimeOffset?> IsOptionalTimestamp(this PropertyBuilder<DateTimeOffset?> property)
    {
        return property.HasConversion(
            fromCode => fromCode != null ? WriteTimestampDataOrDie(fromCode.Value, property.Metadata.Name) : (DateTime?)null,
            fromData => fromData != null ? ReadAsTimestampData(fromData.Value) : null
        );
    }
}
