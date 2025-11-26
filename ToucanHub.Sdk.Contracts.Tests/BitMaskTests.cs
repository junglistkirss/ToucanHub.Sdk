using ToucanHub.Sdk.Contracts.Security;

namespace ToucanHub.Sdk.Contracts.Tests;

public class BitMaskTests
{
    [Fact]
    public void Test()
    {
        BitMask read = new(0b001);
        BitMask write = new(0b010);
        BitMask readWrite = new(0b011);

        Assert.False(read.Intersects(write));
        Assert.True(read.Intersects(readWrite));
        Assert.True(write.Intersects(readWrite));
    }
}
