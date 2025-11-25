using System.Text.RegularExpressions;

namespace ToucanHub.Sdk.Contracts.Names;

public static partial class NamingConvention
{
    public const string ColorPattern = "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
    public const string TagPattern = "^(?!.*[._ -]{2})(?!^[._ -])(?!.*[._ -]$)[\\w. -]{1,64}$";
    public const string SlugPattern = "^(?!.*_{2})(?!_)(?!.+_$)\\w{1,128}$";

    [GeneratedRegex(TagPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    public static partial Regex TagRegex();

    [GeneratedRegex(SlugPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    public static partial Regex SlugRegex();

    [GeneratedRegex(ColorPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    public static partial Regex ColorRegex();


}