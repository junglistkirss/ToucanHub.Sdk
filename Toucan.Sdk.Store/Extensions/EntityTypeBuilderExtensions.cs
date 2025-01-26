using Toucan.Sdk.Contracts.Entities;

namespace Toucan.Sdk.Store.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<T> ObjectProperties<T, TId, TRef>(this EntityTypeBuilder<T> e, TryParseInlineStruct<TRef> tryParse,
        int? size = null)
         where T : class, IObjectEntity<TId, TRef>
         where TId : struct
         where TRef : struct
    {
        _ = e.Property(x => x.Created).IsTimestamp();
        _ = e.Property(x => x.LastModified).IsOptionalTimestamp();
        PropertyBuilder<TRef> creator = e.Property(x => x.CreatedBy).IsInlineStruct(tryParse).IsRequired();
        if (size.HasValue)
            creator.HasMaxLength(size.Value);
        PropertyBuilder<TRef?> updater = e.Property(x => x.LastModifiedBy).IsOptionalInlineStruct(tryParse);
        if (size.HasValue)
            updater.HasMaxLength(size.Value);
        return e;
    }
}
