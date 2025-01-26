using Toucan.Sdk.Contracts.Entities;

namespace Toucan.Sdk.Store.Extensions;

public static class OwnedNavigationBuilderExtensions
{
    public static OwnedNavigationBuilder<TOwner, T> OwnObjectProperties<TOwner, T, TId, TRef>(
        this OwnedNavigationBuilder<TOwner, T> e,
        TryParseInlineStruct<TRef> tryParse,
        int? size = null)
        where TOwner : class, IEntity<TId>
        where T : class, IObjectEntity<TId, TRef>
        where TId : struct
        where TRef : struct
    {
        _ = e.Property(x => x.Created).IsRequired();
        _ = e.Property(x => x.LastModified);
        PropertyBuilder<TRef> creator = e.Property(x => x.CreatedBy).IsInlineStruct(tryParse).IsRequired();
        if (size.HasValue) creator.HasMaxLength(size.Value);

        PropertyBuilder<TRef?> updater = e.Property(x => x.LastModifiedBy).IsOptionalInlineStruct(tryParse);
        if (size.HasValue) updater.HasMaxLength(size.Value);
        return e;
    }

    public static OwnedNavigationBuilder<TOwner, T> OwnEtag<TOwner, T, TId>(this OwnedNavigationBuilder<TOwner, T> e, bool? isUnique = true)
        where TOwner : class, IEntity<TId>
        where TId : struct
        where T : class, IHaveEtag
    {
        _ = e.Property(x => x.ETag).IsEtagToken();
        if (isUnique.HasValue) e.HasIndex(x => x.ETag).IsUnique(isUnique.Value);
        return e;
    }
}
