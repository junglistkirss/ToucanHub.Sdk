using System.ComponentModel;
using System.Diagnostics;

namespace Toucan.Sdk.Contracts.Security;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
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
        //string[] parts = path.Split(SeparatorMain);

        ReadOnlyMemory<char> current = path.AsMemory();
        ReadOnlySpan<char> currentSpan = current.Span;

        Part[]? result = new Part[CountOf(currentSpan, SeparatorMain) + 1];

        int j = 0;
        if (result.Length > 1)
        {
            int i = 0;
            while (i < currentSpan.Length)
            {
                if (currentSpan[i] == SeparatorMain)
                {
                    result[j] = Parse(current[..i]);

                    current = current[(i + 1)..];
                    currentSpan = current.Span;

                    i = 0;
                    j++;
                }
                else i++;
            }
        }
        result[j] = Parse(current);
        return result;
    }

    public static Part Parse(ReadOnlyMemory<char> current)
    {
        ReadOnlySpan<char> currentSpan = current.Span;

        if (currentSpan.Length == 0)
            return new Part([], false);

        if (currentSpan.Length == 1 && currentSpan[0] == CharAny)
            return new Part(null, false);

        bool isExclusion = false;

        if (currentSpan[0] == CharExclude)
        {
            isExclusion = true;

            current = current[1..];
            currentSpan = current.Span;
        }

        if (currentSpan.Length == 0)
            return new Part([], isExclusion);

        //if (current.Length > 1 /*|| currentSpan[0] != CharAny*/)
        //{
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
        //}
        //else
        //{
        //    return new Part(null, isExclusion);
        //}
    }

    private static int CountOf(ReadOnlySpan<char> text, char character)
    {
        int count = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == character)
                count++;
        }
        return count;
    }

    public static bool Intersects(ref Part given, ref Part requested, bool allowNull)
    {
        if (given.Alternatives is null)
            return true;

        if (requested.Alternatives is null)
            return allowNull;

        bool shouldIntersect = !(given.Exclusion ^ requested.Exclusion);

        bool isIntersected = false;

        for (int i = 0; i < given.Alternatives.Length; i++)
        {
            for (int j = 0; j < requested.Alternatives.Length; j++)
            {
                if (given.Alternatives[i].Span.Equals(requested.Alternatives[j].Span, StringComparison.OrdinalIgnoreCase))
                {
                    isIntersected = true;
                    break;
                }
            }
        }

        return isIntersected == shouldIntersect;
    }

    private string GetDebuggerDisplay()
    {
        return $"{(Exclusion?"Not":"")}{string.Join(',', Alternatives ?? [])}";
    }
}
