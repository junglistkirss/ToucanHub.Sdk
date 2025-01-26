using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Toucan.Sdk.Store.Extensions;


public static class PropertyBuilderRefExtensions
{
    public static PropertyBuilder<TStruct> IsInlineStruct<TStruct>(this PropertyBuilder<TStruct> property, TryParseInlineStruct<TStruct> parse)
        where TStruct : struct
    {
        return property.HasConversion(
            fromCode => WriteInlineStruct(fromCode),
            fromData => ReadAsInlineStruct(parse, fromData, property.Metadata.Name),
            new ValueComparer<TStruct>(
                (l, r) => l.ToString() == r.ToString(),
                x => x.GetHashCode(),
                x => x
            )
        );
    }
    public static PropertyBuilder<TStruct?> IsOptionalInlineStruct<TStruct>(this PropertyBuilder<TStruct?> property, TryParseInlineStruct<TStruct> parse)
        where TStruct : struct
    {
        return property.HasConversion(
            fromCode => fromCode != null ? WriteInlineStruct(fromCode) : null,
            fromData => fromData != null ? ReadAsInlineStruct(parse, fromData, property.Metadata.Name) : null
        );
    }
    private static string WriteInlineStruct<TStruct>(TStruct fromCode) => fromCode!.ToString()!;
    private static TStruct ReadAsInlineStruct<TStruct>(TryParseInlineStruct<TStruct> parse, string fromData, string name)
        where TStruct : struct => parse(fromData, out TStruct @ref) ? @ref : throw new InvalidOperationException($"Column {name} valid {typeof(TStruct)}");


}



