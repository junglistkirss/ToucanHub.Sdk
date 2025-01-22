namespace Toucan.Sdk.Utils.Tests;

public class HasherUnitTest
{

    [Theory]
    [InlineData("ABC", "abc")]
    [InlineData("Abc", "abc")]
    [InlineData("AbC", "abc")]
    [InlineData("abC", "abc")]
    [InlineData("abc", " abc")]
    public void ToSha256(string value1, string value2)
    {
        string res1 = value1.ToSha256();
        string res2 = value2.ToSha256();
        Assert.NotEqual(res1, res2);
    }

    [Theory]
    [InlineData("ABC", "abc")]
    [InlineData("Abc", "abc")]
    [InlineData("AbC", "abc")]
    [InlineData("abC", "abc")]
    [InlineData("abc", " abc")]
    public void ToSha512(string value1, string value2)
    {
        string res1 = value1.ToSha512();
        string res2 = value2.ToSha512();
        Assert.NotEqual(res1, res2);
    }
    [Theory]
    [InlineData("ABC", "abc")]
    [InlineData("Abc", "abc")]
    [InlineData("AbC", "abc")]
    [InlineData("abC", "abc")]
    [InlineData("abc", " abc")]
    public void ToMD5(string value1, string value2)
    {
        string res1 = value1.ToMD5();
        string res2 = value2.ToMD5();
        Assert.NotEqual(res1, res2);
    }

    [Fact]
    public void New()
    {
        string res1 = Hasher.New();
        string res2 = Hasher.New();
        Assert.NotEqual(res1, res2);
    }

    [Fact]
    public void Random()
    {
        string res1 = Hasher.Randomize("");
        string res2 = Hasher.Randomize("");
        Assert.NotEqual(res1, res2);
        Assert.NotSame(res1, res2);
    }

    [Fact]
    public void Simple()
    {
        string res1 = Hasher.Simple();
        string res2 = Hasher.Simple();
        Assert.NotEqual(res1, res2);
    }
    [Fact]
    public void Hash()
    {
        string res1 = Guid.NewGuid().Hash();
        string res2 = Guid.NewGuid().Hash();
        Assert.NotEqual(res1, res2);
    }

}
