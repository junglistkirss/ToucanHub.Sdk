using static Toucan.Sdk.Utils.TextExtensions;

namespace Toucan.Sdk.Utils.Tests;

public class TextUnitTest
{
    [Theory]
    [InlineData("", 0, true)] // Empty string
    [InlineData("a", 0, true)] // Single character, index 0
    [InlineData("a", 1, false)] // Single character, out-of-bounds index
    [InlineData("abc", -1, false)] // Negative index
    [InlineData("abc", 3, false)] // Index out of range (equals length)

    // Test cases for newline and CR-LF boundary
    [InlineData("a\nb", 0, true)] // Between 'a' and '\n'
    [InlineData("a\nb", 1, true)] // Between '\n' and 'b'
    [InlineData("a\r\nb", 0, true)] // Between 'a' and '\r'
    [InlineData("a\r\nb", 1, false)] // Between '\r' and '\n' (should not be a boundary)
    [InlineData("a\r\nb", 2, true)] // Between '\n' and 'b'

    // Test cases for mid-letter and mid-numeric boundaries
    [InlineData("A.B", 1, false)] // Period between letters
    [InlineData("A.B", 2, true)] // After period
    [InlineData("123-456", 3, false)] // Hyphen between numbers
    [InlineData("123-456", 4, false)] // After hyphen
    [InlineData("123456", 3, false)] // No boundary in numeric sequence
    [InlineData("A B", 1, true)] // Space between letters

    // Test cases for Katakana characters
    [InlineData("\u30A2\u30A4", 0, false)] // Two adjacent Katakana characters
    [InlineData("\u30A2 a", 0, true)] // Katakana followed by space and letter

    public void TestIsBoundary(string value, int index, bool expected)
    {
        ReadOnlySpan<char> span = value.AsSpan();
        bool result = WordBoundary.IsBoundary(span, index);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("ABC", 1)]
    [InlineData("AB bc", 2)]
    [InlineData("AB.bc", 1)]
    [InlineData("AB:bc", 1)]
    [InlineData("AB-bc", 2)]
    [InlineData("AB.B Bccc", 2)]
    [InlineData("AB. 123", 2)]
    public void WordCount(string value, int expected)
    {
        int result = value.WordCount();
        Assert.Equal(expected, result);
    }


    [Theory]
    [InlineData("", 0)]
    [InlineData("ABC", 3)]
    [InlineData("AB bc", 4)]
    [InlineData("AB.bc", 4)]
    [InlineData("AB-12", 4)]
    [InlineData("AB.B 123", 6)]
    [InlineData("AB. 0", 3)]
    public void CharacterCount(string value, int expected)
    {
        int result = value.CharacterCount();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("ABC", 3)]
    [InlineData("AB bc", 4)]
    [InlineData("AB bc.", 5)]
    [InlineData("AB-12", 5)]
    [InlineData("AB!12", 5)]
    [InlineData("AB:12", 5)]
    [InlineData("AB.B 123", 7)]
    [InlineData("AB. 0", 4)]
    public void CharacterCountWithPunctuation(string value, int expected)
    {
        int result = value.CharacterCount(true);
        Assert.Equal(expected, result);
    }
}
