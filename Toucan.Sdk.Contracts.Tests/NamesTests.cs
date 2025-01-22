using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Tests;

public class NamesTests
{
    [Fact]
    public void SlugEquals()
    {
        Assert.Equal(Slug.Parse("TEST"), Slug.Create("test"));
        Assert.Equal(Slug.Parse("TEST-A"), Slug.Create("test-a"));
        Assert.Equal(Slug.Parse("TEST.2"), Slug.Create("test.2"));
        Assert.Equal(Slug.Parse("test"), Slug.Create("TEST"));
        Assert.Equal(Slug.Parse(" test"), Slug.Create("   TEsT   "));
        Assert.Equal(Slug.Parse("TesT"), Slug.Parse(" TesT "));
    }

    [Fact]
    public void SlugInvalid()
    {
        Assert.ThrowsAny<Exception>(() => Slug.Parse("TEST."));
        Assert.ThrowsAny<Exception>(() => Slug.Parse("TEST-"));
        Assert.ThrowsAny<Exception>(() => Slug.Parse("TEST_"));
        Assert.ThrowsAny<Exception>(() => Slug.Parse("TE..ST"));
        Assert.ThrowsAny<Exception>(() => Slug.Parse("TE--ST"));
        Assert.ThrowsAny<Exception>(() => Slug.Parse("TE__ST"));
        Assert.ThrowsAny<Exception>(() => Slug.Parse(".TEST"));
        Assert.ThrowsAny<Exception>(() => Slug.Parse("-TEST"));
        Assert.ThrowsAny<Exception>(() => Slug.Parse("_TEST"));
    }

    [Theory]
    [InlineData("TESTS", "tests")]
    [InlineData("ab cd ef", "AB cd EF")]
    [InlineData("  AB CD EF  ", "AB CD EF")]
    [InlineData("  AB-CD-EF  ", "ab-cd-ef")]
    public void TagEquals(string input, string expected)
    {
        Tag l = Tag.Parse(input);
        Tag r = Tag.Parse(expected);
        Assert.Equal(l, r);
        Assert.True(l.Equals(expected));
        Assert.True(r.Equals(expected));
        Assert.NotEqual(input, expected);

    }

    [Fact]
    public void ColorEquals()
    {
        Assert.Equal(Color.Parse("#FFF"), Color.Create("#fff"));
        Assert.Equal("#FFF", Color.Parse("#FFF").HexCode );
        Assert.Equal("#fff", Color.Parse("#fff").HexCode );

        Assert.Equal(Color.Parse("#FFF000"), Color.Create("#fff000"));
        Assert.Equal("#FFF000", Color.Parse("#FFF000").HexCode);
        Assert.Equal("#fff000", Color.Parse("#fff000").HexCode);
    }

}
