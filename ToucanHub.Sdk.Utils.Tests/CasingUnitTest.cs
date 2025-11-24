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