using System.Text.RegularExpressions;

namespace Toucan.Sdk.Contracts.Names;

public static partial class NamingConvention
{
    /// <summary>
    /// De 1 à 100 caractères contenant des lettres, chiffres, points, underscores, tirets, ou espaces
    /// <para>
    /// Sans que ces symboles ou espaces soient consécutifs, ni en début ni en fin
    /// </para>
    /// </summary>
    public const string ColorPattern = "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";

    /// <summary>
    /// De 1 à 100 caractères contenant des lettres, chiffres, points, underscores, tirets, ou espaces
    /// <para>
    /// Sans que ces symboles ou espaces soient consécutifs, ni en début ni en fin
    /// </para>
    /// </summary>
    public const string TagPattern = "^(?!.*[._ -]{2})(?!^[._ -])(?!.*[._ -]$)[\\w. -]{1,100}$";

    /// <summary>
    /// De 1 à 64 caractères contenant des lettres, chiffres, points, underscores, ou tirets
    /// <para>
    /// Sans que ces symboles soient consécutifs, ni en début ni en fin
    /// </para>
    /// </summary>
    public const string SlugPattern = "^(?!.*[._-]{2})(?!^[._-])(?!.*[._-]$)[\\w.-]{1,64}$";

    [GeneratedRegex(TagPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    public static partial Regex TagRegex();

    [GeneratedRegex(SlugPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    public static partial Regex SlugRegex();

    [GeneratedRegex(ColorPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    public static partial Regex ColorRegex();


}