using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Extensions;

public static partial class SlugExtensions
{
    public static Slug ToSlugName(this string name) => Slug.Create(name);

    public static Slug[] ToSlugs(this string[] names) => [.. names.Select(x => x.ToSlugName())];

    public static Slug[] ToSlugsOrEmpty(this string[]? names) => names?.Select(x => x.ToSlugName()).ToArray() ?? Array.Empty<Slug>()!;

    public static bool IsEmpty(this Slug input) => input == Slug.Empty;

}
