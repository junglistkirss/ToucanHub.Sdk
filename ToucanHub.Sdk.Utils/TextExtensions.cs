namespace ToucanHub.Sdk.Utils;

public static class TextExtensions
{
    public static class WordBoundary
    {
        private const string NewLines = "\n\v\f\r\u0085\u2028\u2029";
        private const string MidLetter = ":.'’·״‧︓﹕：";
        private const string MidNumeric = "+-*/=<>×÷";

        public static bool IsBoundary(ReadOnlySpan<char> value, int index)
        {
            if (value.Length == 0)
                return true;

            if (index < 0 || index >= value.Length)
                return false;

            char c = value[index];

            if (index == value.Length - 1)
                return true;

            char cNext = value[index + 1];

            // CRLF
            if (c == '\r' && cNext == '\n') return false;

            // look ahead
            if (index < value.Length - 2)
            {
                char cNext2 = value[index + 2];

                if (IsNonCJKLetter(c) && IsMidLetter(cNext) && IsNonCJKLetter(cNext2))
                    return false;

                if (char.IsDigit(c) && IsMidNumeric(cNext) && char.IsDigit(cNext2))
                    return false;
            }

            // look behind
            if (index > 0)
            {
                char cPrev = value[index - 1];

                if (IsMidLetter(c) && IsNonCJKLetter(cNext) && IsNonCJKLetter(cPrev))
                    return false;

                if (IsMidNumeric(c) && char.IsDigit(cPrev) && char.IsDigit(cNext))
                    return false;
            }

            // same type letters/digits
            if (IsNumberOrLetter(c) && IsNumberOrLetter(cNext))
                return false;

            if (IsNewLine(c) || IsNewLine(cNext))
                return true;

            if (IsKatakana(c) && IsKatakana(cNext))
                return false;

            return true;
        }
        private static bool IsNewLine(char c) => NewLines.IndexOf(c) >= 0;
        private static bool IsMidLetter(char c) => MidLetter.IndexOf(c) >= 0;
        private static bool IsMidNumeric(char c) => MidNumeric.IndexOf(c) >= 0 || char.GetUnicodeCategory(c) == UnicodeCategory.MathSymbol;

        private static bool IsNumberOrLetter(char c) => char.IsDigit(c) || IsNonCJKLetter(c);

        private static bool IsNonCJKLetter(char c) => char.IsLetter(c) && char.GetUnicodeCategory(c) != UnicodeCategory.OtherLetter;

        private static bool IsKatakana(char c) => c >= '゠' && c <= 'ヿ';
    }

    public static int WordCount(this ReadOnlySpan<char> value)
    {
        if (value.Length == 0) return 0;

        int count = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];

            if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c) && WordBoundary.IsBoundary(value, i))
                count++;
        }
        return count;
    }

    public static int WordCount(this string? value) => value?.AsSpan().WordCount() ?? 0;

    public static int CharacterCount(this ReadOnlySpan<char> value, bool withPunctuation = false)
    {
        int count = 0;
        foreach (char c in value)
        {
            if (char.IsLetterOrDigit(c) || (withPunctuation && char.IsPunctuation(c)))
                count++;
        }
        return count;
    }

    public static int CharacterCount(this string? value, bool withPunctuation = false) => value?.AsSpan().CharacterCount(withPunctuation) ?? 0;
}