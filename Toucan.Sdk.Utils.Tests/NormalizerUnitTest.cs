namespace Toucan.Sdk.Utils.Tests;

public class NormalizerUnitTest
{
    [Theory]
    [InlineData("3.14", "3.14")]
    [InlineData("◌ABC", "ABC")]
    [InlineData("1é", "1e")]
    [InlineData("à", "a")]
    [InlineData("@", "@")]
    public void RemoveDiacritics(string? test, string? expected)
    {
        string? result = NameNormalizer.RemoveDiacritics(test);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("3.14", "3.14")]
    [InlineData("Sébastian", "Sebastian")]
    [InlineData("ùùù", "uuu")]
    public void NormalizeName(string? test, string? expected)
    {
        string? result = NameNormalizer.NormalizeName(test);
        Assert.Equal(expected, result);
    }
}
