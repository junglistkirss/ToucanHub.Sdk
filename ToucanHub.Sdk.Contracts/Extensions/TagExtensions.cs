using ToucanHub.Sdk.Contracts.Names;

namespace ToucanHub.Sdk.Contracts.Extensions;


public static class TagExtensions
{

    public static Tag[] ToTags(this string[] names) => [.. names.Select(Tag.Parse)];
}
