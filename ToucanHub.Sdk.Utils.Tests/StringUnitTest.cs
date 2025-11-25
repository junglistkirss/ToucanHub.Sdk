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
    [InlineData(null, false, null)]
    [InlineData("", false, null)]
    [InlineData("  ", false, null)]
    [InlineData(" aa  ", true, "aa")]
    public void NullTrimmed(string? value, bool expected, string? result)
    {
        Assert.Equal(expected, value.IsNotNullTrimmed(out string? notNullTrimmed));
        Assert.Equal(result, notNullTrimmed);

        Assert.Equal(!expected, value.IsNullTrimmed(out string? nullTrimmed));
        Assert.Equal(result, nullTrimmed);
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
