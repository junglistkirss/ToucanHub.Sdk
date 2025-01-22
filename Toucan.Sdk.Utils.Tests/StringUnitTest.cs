namespace Toucan.Sdk.Utils.Tests;
public class StringUnitTest
{

    [Theory]
    [InlineData("[a-Z{1,-2}]", false)]
    [InlineData("\\W\\]", true)]
    public void IsValidRegex(string value, bool expected)
    {
        bool result = value.IsValidRegex();
        Assert.Equal(expected, result);
    }


    [Theory]
    [InlineData("plouf.com", false)]
    [InlineData("plouf@com", false)]
    [InlineData(" @ploufcom", false)]
    [InlineData("tot@plouf.nope", true)]
    public void IsEmail(string value, bool expected)
    {
        bool result = value.IsEmail();
        Assert.Equal(expected, result);
    }


    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("tot@plouf.nope", true)]
    public void IsNotNulOrEmptOrWithespace(string? value, bool expected)
    {
        bool result = value.IsNotNullOrWhiteSpace();
        Assert.Equal(expected, result);
    }

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
