using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Extensions;

public static partial class DomainIdExtensions
{
    public static DomainId[] ToDomainIds(this Slug[] names) => [.. names.Select(DomainId.FromSlug)];
    public static DomainId[] ToDomainIds(this string[] names) => [.. names.Select(DomainId.Parse)];

    public static DomainId[] ToDomainIdsOrEmpty(this Slug[]? names) => names?.Select(DomainId.FromSlug).ToArray() ?? Array.Empty<DomainId>()!;
}
