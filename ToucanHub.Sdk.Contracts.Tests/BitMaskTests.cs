using ToucanHub.Sdk.Contracts.Security;

namespace ToucanHub.Sdk.Contracts.Tests;

public class BitMaskTests
{
    [Fact]
    public void Test()
    {
        var read = new BitMask(0b001);
        var write = new BitMask(0b010);
        var readWrite = new BitMask(0b011);

        Assert.False(read.Intersects(write));
        Assert.True(read.Intersects(readWrite));
        Assert.True(write.Intersects(readWrite));
    }
}
