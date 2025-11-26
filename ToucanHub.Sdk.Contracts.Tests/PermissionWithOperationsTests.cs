using ToucanHub.Sdk.Contracts.Security;

namespace ToucanHub.Sdk.Contracts.Tests;

public class PermissionWithOperationsTests
{
    // --- Parsing des opérations multiples ---
    [Theory]
    [InlineData("customer.42@read", "customer.42", "read")]
    [InlineData("customer.42@read,write", "customer.42", "read,write")]
    [InlineData("customer.*@read,write,delete", "customer.*", "read,write,delete")]
    [InlineData("customer.^one@read,write,delete", "customer.^one", "read,write,delete")]
    [InlineData("order.5.item@delete,write", "order.5.item", "delete,write")]
    [InlineData("service.12@*", "service.12", "*")]
    public void Permission_ParsesOperationsCorrectly(string input, string expectedScope, string expectedOps)
    {
        var perm = new Permission(input);

        Assert.Equal(expectedScope, string.Join('.', perm.Scope));
        Assert.Equal(
            expectedOps.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLowerInvariant()),
            perm.Operations
        );
    }

    // --- Validation des opérations ---
    [Theory]
    [InlineData("customer.42@read")]
    [InlineData("customer.42@read,write")]
    [InlineData("customer.*@read,write,delete")]
    public void Permission_OperationsAreValid(string input)
    {
        string[] AllowedOps = ["read", "write", "delete"];
        var perm = new Permission(input); // ne doit pas throw
        Assert.All(perm.Operations, op => AllowedOps.Contains(op));
    }

    // --- Allows avec opérations multiples ---
    [Theory]
    [InlineData("customer.42@read,write", "customer.42@read", true)]
    [InlineData("customer.42@read,write", "customer.42@write", true)]
    [InlineData("customer.42@read,write", "customer.42@delete", false)]
    [InlineData("customer.*@read,write", "customer.123@read", true)]
    [InlineData("customer.*@read,write", "customer.123@delete", false)]
    [InlineData("customer.42@*", "customer.42@read", true)]
    [InlineData("customer.42@*", "customer.42@write,delete", true)]
    [InlineData("customer.42@read", "customer.42@read,write", false)]
    public void Permission_AllowsOperationsCorrectly(string permStr, string requestedStr, bool expected)
    {
        var perm = new Permission(permStr);
        var requested = new Permission(requestedStr);

        Assert.Equal(expected, perm.Allows(requested));
    }

    // --- Includes (PartialCovers) avec scope seulement ---
    [Theory]
    [InlineData("customer.42@read,write", "customer.42@read", true)]
    [InlineData("customer.*@read,write", "customer.123@delete", true)] // inclusion scope ok même si opérations diffèrent
    [InlineData("customer.42@read", "customer.42@write", true)] // inclusion scope ok
    [InlineData("customer.42@read", "customer.43@read", false)]
    public void Permission_IncludesScopeCorrectly(string permStr, string requestedStr, bool expected)
    {
        var perm = new Permission(permStr);
        var requested = new Permission(requestedStr);

        Assert.Equal(expected, perm.Includes(requested));
    }
}
