using ToucanHub.Sdk.Contracts.Names;

namespace ToucanHub.Sdk.Contracts.Extensions;


public static class TagExtensions
{
    public static Tag ToTagName(this string name) => new(name);

    public static Tag[] ToTags(this string[] names) => [.. names.Select(x => x.ToTagName())];

    public static Tag[] ToTagsOrEmpty(this string[]? names) => names?.Select(x => x.ToTagName()).ToArray() ?? Array.Empty<Tag>()!;





}
