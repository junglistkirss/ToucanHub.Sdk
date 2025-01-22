using System.Text;

namespace Toucan.Sdk.Utils;

public static class NameNormalizer
{
    public static string? RemoveDiacritics(string? text)
    {
        if (text == null)
            return null;

        string normalizedString = text.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new();

        foreach (char c in normalizedString)
        {
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory == UnicodeCategory.UppercaseLetter
                || unicodeCategory == UnicodeCategory.LowercaseLetter
                || unicodeCategory == UnicodeCategory.DecimalDigitNumber
                || unicodeCategory == UnicodeCategory.DashPunctuation
                || unicodeCategory == UnicodeCategory.ConnectorPunctuation
                || unicodeCategory == UnicodeCategory.MathSymbol
                || unicodeCategory == UnicodeCategory.OtherPunctuation)
                _ = stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
    public static string? NormalizeName(params string?[] input)
    {
        if (input != null)
        {
            string i = string.Join('_', input.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x?.Trim()));
            return RemoveDiacritics(i);
        }
        throw new ArgumentNullException(nameof(input));
    }
}
