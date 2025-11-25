using ToucanHub.Sdk.Contracts.Names;

namespace ToucanHub.Sdk.Contracts.Extensions;

public static partial class SlugExtensions
{
    public static Slug[] ToSlugs(this string[] names) => [.. names.Select(Slug.Create)];

    public static bool IsEmpty(this Slug input) => input == Slug.Empty;

}
