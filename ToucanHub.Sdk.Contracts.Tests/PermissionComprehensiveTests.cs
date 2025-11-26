using ToucanHub.Sdk.Contracts.Security;

namespace ToucanHub.Sdk.Contracts.Tests;

public class PermissionComprehensiveTests
{
    [Theory]
    [InlineData("customer.42@read,write", "customer.42@read", true)]
    [InlineData("customer.42@read,write", "customer.42@write", true)]
    [InlineData("customer.42@read,write", "customer.42@delete", false)]
    [InlineData("customer.*@read,write", "customer.123@read", true)]
    [InlineData("customer.*@read,write", "customer.123@write", true)]
    [InlineData("customer.*@read,write", "customer.123@delete", false)]
    [InlineData("customer.42@*", "customer.42@read", true)]
    [InlineData("customer.42@*", "customer.42@write,delete", true)]
    [InlineData("customer.42@read", "customer.42@read,write", false)]
    [InlineData("order.*@read,write", "order.5.item@read", true)]
    [InlineData("order.*@read,write", "order.5.item@delete", false)]
    [InlineData("service.*@read,write,delete", "service.12@delete", true)]
    [InlineData("service.*@read,write,delete", "service.12@read,write", true)]
    [InlineData("service.*@read,write,delete", "service.12@execute", false)]
    [InlineData("customer.42@read,write", "customer.43@read", false)]
    [InlineData("customer.42@read,write", "customer.42.sub@read", true)]
    public void Allows_Combinations(string permStr, string requestedStr, bool expected)
    {
        Permission perm = new(permStr);
        Permission requested = new(requestedStr);

        Assert.Equal(expected, perm.Allows(requested));
    }

    [Theory]
    [InlineData("customer.42@read,write", "customer.42@read", true)]
    [InlineData("customer.*@read,write", "customer.123@delete", true)]
    [InlineData("customer.42@read", "customer.42@write", true)]
    [InlineData("customer.42@read", "customer.43@read", false)]
    [InlineData("customer.42@read,write", "customer.42.sub@read", true)]
    [InlineData("customer.*@read,write", "customer.42.sub@write", true)]
    [InlineData("order.*@read,write", "order.5.item@read", true)]
    [InlineData("order.*@read,write", "order.5.item@delete", true)]
    [InlineData("service.*@read,write,delete", "service.12@execute", true)]
    [InlineData("customer.42@*", "customer.42@read", true)]
    [InlineData("customer.42@*", "customer.42.sub@write", true)]
    public void Includes_Combinations(string permStr, string requestedStr, bool expected)
    {
        Permission perm = new(permStr);
        Permission requested = new(requestedStr);

        Assert.Equal(expected, perm.Includes(requested));
    }

    [Theory]
    [InlineData("customer.42@read", "customer.42", "read")]
    [InlineData("customer.42@read,write", "customer.42", "read,write")]
    [InlineData("customer.*@read,write,delete", "customer.*", "read,write,delete")]
    [InlineData("order.5.item@delete,write", "order.5.item", "delete,write")]
    [InlineData("service.12@*", "service.12", "*")]
    [InlineData("customer.42.sub@read,write,delete", "customer.42.sub", "read,write,delete")]
    [InlineData("order.*@read", "order.*", "read")]
    public void ParsesOperationsCorrectly(string input, string expectedScope, string expectedOps)
    {
        Permission perm = new(input);

        // Scope vérifié sur la chaîne brute (part)
        Assert.Equal(expectedScope, perm.Scope);

        // Vérifie les opérations
        Assert.Equal(
            expectedOps.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLowerInvariant()),
            perm.Operations.Select(x => x.ToString())
        );
    }
}
