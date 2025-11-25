namespace ToucanHub.Sdk.Utils.Tests;

public class CasingUnitTest
{
    [Fact]
    public void TestPascalCase()
    {
        Assert.Equal("", string.Empty.ToPascalCase());
        Assert.Equal("Aa", "_- aa".ToPascalCase());
        Assert.Equal("Aa", "aa".ToPascalCase());
        Assert.Equal("AA", "a a".ToPascalCase());
        Assert.Equal("AA", "a-a".ToPascalCase());
        Assert.Equal("AaaAaa", "aaa_aaa".ToPascalCase());
    }

    [Fact]
    public void TestCamelCase()
    {
        Assert.Equal("", string.Empty.ToCamelCase());
        Assert.Equal("aa", "_- aa".ToCamelCase());
        Assert.Equal("aa", "Aa".ToCamelCase());
        Assert.Equal("aA", "a a".ToCamelCase());
        Assert.Equal("aA", "A-A".ToCamelCase());
        Assert.Equal("aaaAaa", "aaa_aaa".ToCamelCase());
        Assert.Equal("aaaAaa", "_aaa_aaa".ToCamelCase());
    }

    [Fact]
    public void TestKebabCase()
    {
        Assert.Equal("", string.Empty.ToKebabCase());
        Assert.Equal("aa", "_- aa".ToKebabCase());
        Assert.Equal("aa", "Aa".ToKebabCase());
        Assert.Equal("a-a", "a a".ToKebabCase());
        Assert.Equal("a-a", "a_a".ToKebabCase());
        Assert.Equal("aaa-aaa", "  aaa_aaa".ToKebabCase());
        Assert.Equal("aaa-aaa", "_aaa aaa".ToKebabCase());
    }
}

public class SizeUnitTest
{
    [Theory]
    [InlineData(0, "0 B")]
    [InlineData(1, "1 B")]
    [InlineData(512, "512 B")]
    [InlineData(1023, "1023 B")]
    [InlineData(1024, "1 kB")]
    [InlineData(1536, "2 kB")]            // 1536 / 1024 = 1.5
    [InlineData(2048, "2 kB")]
    [InlineData(1048576, "1 MB")]         // 1024 * 1024
    [InlineData(1073741824, "1 GB")]      // 1024^3
    public void WithoutPrecision(int value, string response)
    {
        Assert.Equal(response, value.ToReadableSize());
    }

    [Theory]
    [InlineData(1099511627776, "1 TB")]   // 1024^4
    [InlineData(1125899906842624, "1 PB")]
    [InlineData(1125899906842624 * 2, "2 PB")]
    [InlineData(long.MaxValue, "8 EB")]
    public void LargeWithoutPrecision(long value, string response)
    {
        Assert.Equal(response, value.ToReadableSize());
    }

    [Theory]
    [InlineData(1536, "1.5 kB", 1)]
    [InlineData(1572864, "1.5 MB", 1)]   // 1.5 MiB
    public void WithPrecision(int value, string response, int precision)
    {
        Assert.Equal(response, value.ToReadableSize(precision));
    }

    [Theory]
    [InlineData(-1, "")]
    public void Negative(int value, string response)
    {
        Assert.Equal(response, value.ToReadableSize());
    }
}