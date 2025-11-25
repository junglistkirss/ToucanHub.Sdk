namespace ToucanHub.Sdk.Utils.Tests;

public class NormalizerUnitTest
{
    [Theory]
    [InlineData("3.14", "3.14")]
    [InlineData("◌ABC", "ABC")]
    [InlineData("1é", "1e")]
    [InlineData("à", "a")]
    [InlineData("@", "@")]
    [InlineData(null, null)]
    public void RemoveDiacritics(string? test, string? expected)
    {
        string? result = Normalizer.RemoveDiacritics(test);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("3.14", "3.14")]
    [InlineData("Sébastian", "Sebastian")]
    [InlineData("ùùù", "uuu")]
    public void NormalizeName(string? test, string? expected)
    {
        string? result = Normalizer.NormalizeName(test);
        Assert.Equal(expected, result);
    }
}
