namespace ToucanHub.Sdk.Utils.Tests;
public class StringUnitTest
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("   ", null)]
    [InlineData("  zero ", "zero")]
    public void TrimOrNull(string? value, string? expected)
    {
        string? result = value.TrimOrNull();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null, "resp", "resp")]
    [InlineData("00", "resp", "00")]
    [InlineData("  00   ", "resp", "00")]
    public void Or(string? value, string or, string expected)
    {
        string result = value.Or(or);
        Assert.Equal(expected, result);
    }
}
