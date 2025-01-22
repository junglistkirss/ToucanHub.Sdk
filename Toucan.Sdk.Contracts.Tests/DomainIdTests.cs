using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Tests;
public class DomainIdTests
{
    [Fact]
    public void DomainIdEquals()
    {
        Assert.Equal(DomainId.Empty, default);
        Assert.Equal(DomainId.Parse("TEST"), DomainId.Parse("test"));
        Assert.Equal(DomainId.Parse("test"), DomainId.Parse("TEST"));
        Assert.Equal(DomainId.Parse(" test"), DomainId.Parse("   TEST   "));
    }
    [Fact]

    public void DomainIdEquals2()
    {
        Assert.Equal(3, DomainId.Parse(" test ~  DOMAIN ~ 42 ").Parts.Length);
        Assert.Equal(DomainId.Parse(" test ~  DOMAIN ~ 42 "), DomainId.Parse("teST~DOMain~42"));
        Assert.Equal("test~domain~42", DomainId.Parse(" test ~  DOMAIN ~ 42 ").Id);
    }
}
