using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Extensions;

public static partial class DomainIdExtensions
{
    //public static DomainId[] Adapt<T>(this DomainId<T>[] names) => names.Select(x => x.Id).ToArray();

    public static DomainId[] ToDomainIds(this Slug[] names) => names.Select(DomainId.FromSlug).ToArray();
    public static DomainId[] ToDomainIds(this string[] names) => names.Select(DomainId.Parse).ToArray();

    public static DomainId[] ToDomainIdsOrEmpty(this Slug[]? names) => names?.Select(DomainId.FromSlug).ToArray() ?? Array.Empty<DomainId>()!;

    //public static string ValidDomainId(this string input)
    //{
    //    if (input == null || string.IsNullOrWhiteSpace(input))
    //        return DomainId.Empty;

    //    input = input.Trim();

    //    if (input.Length > 4000)
    //        throw new ArgumentException($"{nameof(input)} is not a valid slug : too long", nameof(input));

    //    if (NamingConvention.DomainIdRegex().IsMatch(input))
    //        return input;
    //    throw new InvalidDataException($"{nameof(input)} is not a valid slug : {input}");
    //}

    //public static bool IsValidDomainId(this string input, out string? slug)
    //{
    //    slug = null;
    //    try
    //    {
    //        slug = input.ValidDomainId();
    //        return true;
    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }
    //}
}
