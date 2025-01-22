namespace Toucan.Sdk.Utils;
public static class TextExtensions
{

    public static class WordBoundary
    {
        private static readonly HashSet<char> NewLines =
        [
            '\n',
            '\v',
            '\f',
            '\r',
            '\u0085',
            '\u2028',
            '\u2029'
        ];

        private static readonly HashSet<char> MidLetter =
        [
            ':',
            '.',
            '’',
            '\'',
            '·',
            '״',
            '‧',
            '︓',
            '﹕',
            '：'
        ];

        public static bool IsBoundary(ReadOnlySpan<char> value, int index)
        {
            if (value.Length == 0)
                return true;

            if (index < 0 || index > value.Length - 1)
                return false;

            char c = value[index];
            if (index == value.Length - 1)
                return true;

            char c2 = value[index + 1];
            if (c == '\r' && c2 == '\n')
                return false;

            if (index < value.Length - 2)
            {
                char c3 = value[index + 2];
                if (IsNonCJKLetter(c) && IsMidLetter(c2) && IsNonCJKLetter(c3))
                    return false;

                if (char.IsNumber(c) && IsMidNumeric(c2) && char.IsNumber(c3))
                    return false;
            }

            if (index > 0)
            {
                char c4 = value[index - 1];
                if (IsMidLetter(c) && IsNonCJKLetter(c2) && IsNonCJKLetter(c4))
                    return false;

                if (IsMidNumeric(c) && char.IsNumber(c4) && char.IsNumber(c2))
                    return false;
            }

            if (IsNumberOrLetter(c) && IsNumberOrLetter(c2))
                return false;

            if (IsNewLine(c))
                return true;

            if (IsNewLine(c2))
                return true;

            if (IsKatakana(c) && IsKatakana(c2))
                return false;

            return true;
        }

        private static bool IsNewLine(char c) => NewLines.Contains(c);

        private static bool IsMidLetter(char c) => MidLetter.Contains(c);

        private static bool IsNumberOrLetter(char c)
        {
            if (!char.IsNumber(c))
                return IsNonCJKLetter(c);

            return true;
        }

        private static bool IsMidNumeric(char c)
        {
            if (!char.IsPunctuation(c))
                return IsMathSymbol(c);

            return true;
        }

        private static bool IsMathSymbol(char c) => char.GetUnicodeCategory(c) == UnicodeCategory.MathSymbol;

        private static bool IsNonCJKLetter(char c)
        {
            if (char.IsLetter(c))
                return char.GetUnicodeCategory(c) != UnicodeCategory.OtherLetter;

            return false;
        }

        private static bool IsKatakana(char c)
        {
            if (c >= '゠')
                return c <= 'ヿ';

            return false;
        }
    }
    public static int WordCount(this string value) => value?.AsSpan().WordCount() ?? 0;

    public static int WordCount(this ReadOnlySpan<char> value)
    {
        if (value.Length == 0)
            return 0;

        int num = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c) && WordBoundary.IsBoundary(value, i))
                num++;
        }

        return num;
    }
    public static int CharacterCount(this string value, bool withPunctuation = false) => value.AsSpan().CharacterCount(withPunctuation);

    public static int CharacterCount(this ReadOnlySpan<char> value, bool withPunctuation = false)
    {
        int num = 0;
        foreach (char c in value)
        {
            if (char.IsLetterOrDigit(c))
                num++;
            else if (withPunctuation && char.IsPunctuation(c))
                num++;
        }

        return num;
    }
}

