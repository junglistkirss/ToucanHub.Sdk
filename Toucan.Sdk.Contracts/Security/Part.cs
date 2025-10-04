namespace Toucan.Sdk.Contracts.Security;

internal readonly struct Part(ReadOnlyMemory<char>[]? alternatives, bool exclusion)
{
    private const char SeparatorAlternative = '|';
    private const char SeparatorMain = '.';
    private const char CharAny = '*';
    private const char CharExclude = '^';

    public readonly ReadOnlyMemory<char>[]? Alternatives = alternatives;

    public readonly bool Exclusion = exclusion;

    public static Part[] ParsePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return [];

        ReadOnlyMemory<char> current = path.AsMemory();
        ReadOnlySpan<char> currentSpan = current.Span;

        Part[]? result = new Part[CountOf(currentSpan, SeparatorMain) + 1];

        if (result.Length == 1)
            result[0] = Parse(current);
        else
        {
            for (int i = 0, j = 0; i < currentSpan.Length; i++)
            {
                if (currentSpan[i] == SeparatorMain)
                {
                    result[j] = Parse(current[..i]);

                    current = current[(i + 1)..];
                    currentSpan = current.Span;

                    i = 0;
                    j++;
                }
                else if (i == currentSpan.Length - 1 || currentSpan[i] == SeparatorMain)
                {
                    result[j] = Parse(current);
                }
            }
        }

        return result;
    }

    public static Part Parse(ReadOnlyMemory<char> current)
    {
        ReadOnlySpan<char> currentSpan = current.Span;

        if (currentSpan.Length == 0)
            return new Part([], false);

        bool isExclusion = false;

        if (currentSpan[0] == CharExclude)
        {
            isExclusion = true;

            current = current[1..];
            currentSpan = current.Span;
        }

        if (currentSpan.Length == 0)
            return new Part([], isExclusion);

        if (current.Length > 1 || currentSpan[0] != CharAny)
        {
            ReadOnlyMemory<char>[]? alternatives = new ReadOnlyMemory<char>[CountOf(currentSpan, SeparatorAlternative) + 1];

            if (alternatives.Length == 1)
                alternatives[0] = current;
            else
            {
                for (int i = 0, j = 0; i < current.Length; i++)
                {
                    if (currentSpan[i] == SeparatorAlternative)
                    {
                        alternatives[j] = current[..i];

                        current = current[(i + 1)..];
                        currentSpan = current.Span;

                        i = 0;
                        j++;
                    }
                    else if (i == current.Length - 1)
                    {
                        alternatives[j] = current;
                    }
                }
            }

            return new Part(alternatives, isExclusion);
        }
        else
        {
            return new Part(null, isExclusion);
        }
    }

    private static int CountOf(ReadOnlySpan<char> text, char character)
    {
        int length = text.Length;

        int count = 0;

        for (int i = 0; i < length; i++)
        {
            if (text[i] == character)
                count++;
        }

        return count;
    }

    public static bool Intersects(ref Part lhs, ref Part rhs, bool allowNull)
    {
        if (lhs.Alternatives == null)
            return true;

        if (rhs.Alternatives == null)
            return allowNull;

        bool shouldIntersect = !(lhs.Exclusion ^ rhs.Exclusion);

        bool isIntersected = false;

        for (int i = 0; i < lhs.Alternatives.Length; i++)
        {
            for (int j = 0; j < rhs.Alternatives.Length; j++)
            {
                if (lhs.Alternatives[i].Span.Equals(rhs.Alternatives[j].Span, StringComparison.OrdinalIgnoreCase))
                {
                    isIntersected = true;
                    break;
                }
            }
        }

        return isIntersected == shouldIntersect;
    }
}
