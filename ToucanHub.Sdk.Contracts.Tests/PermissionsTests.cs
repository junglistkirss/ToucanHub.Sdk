using ToucanHub.Sdk.Contracts.Security;

namespace ToucanHub.Sdk.Contracts.Tests;

public class PermissionsTests
{
    private const string TestRaw = "Toucan.Test.Assert.{app}.raw.{schema}.can";
    private const string TestAssert = "Test.Assert.Simple";
    private const string TestAssertAny = "Test.Assert.Any.*";
    private const string TestAssertAnyRead = "Test.Assert.Any.read";
    private const string TestAssertSpecBase = "Test.Assert.Spec";
    private const string TestAssertSpecChild = "Test.Assert.Spec.Child";
    private const string TestAssertApp = "Test.Assert.{app}";
    private const string TestAssertAppSchema = "Test.Assert.{app}.Inner.{schema}";

    private PermissionSet CurrentPermissions { get; }

    public PermissionsTests()
    {
        CurrentPermissions = new PermissionSet(
            new Permission(TestRaw),
            new Permission(TestAssert),
            new Permission(TestAssertAny),
            new Permission(TestAssertSpecBase),
            TestAssertApp.For(("app", "MyApp")),
            TestAssertAppSchema.For(("app", "MyApp1|MyApp2"), ("schema", "MySchema1|MySchema2")),
            TestAssertAppSchema.For(("app", "Section"), ("schema", "^MySchema1|MySchema2"))
            );
    }


    [Fact]
    public void TestSimple_Allowed()
    {
        Assert.Multiple(() =>
        {
            Assert.True(CurrentPermissions.Allows(new Permission(TestAssert)));
            Assert.True(CurrentPermissions.Allows(new Permission(TestAssert.ToLower())));
            Assert.True(CurrentPermissions.Allows(new Permission(TestAssert.ToUpper())));
        });
    }

    [Fact]
    public void TestChild_Allowed()
    {
        Assert.Multiple(() =>
        {
            Assert.True(CurrentPermissions.Allows(new Permission($"{TestAssertSpecChild}.Next")));
            Assert.True(CurrentPermissions.Allows(new Permission($"{TestAssertSpecChild}.Next.Sub")));
        });
    }

    [Fact]
    public void TestAny_Allowed()
    {
        Assert.Multiple(() =>
        {
            Assert.True(CurrentPermissions.Allows(new Permission(TestAssertAnyRead)));
            Assert.True(CurrentPermissions.Allows(new Permission($"{TestAssertAnyRead}Plus")));
            Assert.True(CurrentPermissions.Allows(new Permission($"{TestAssertAnyRead}.Sub.Inner")));
        });
    }

    [Fact]
    public void TestOther_NotAllowed()
    {
        Assert.Multiple(() =>
        {
            Assert.False(CurrentPermissions.Allows(new Permission($"Prev{TestAssertSpecBase}")));
            Assert.False(CurrentPermissions.Allows(new Permission($"{TestAssertSpecBase}Next")));
        });
    }

    [Fact]
    public void TestOther_Allowed()
    {
        Assert.Multiple(() =>
        {
            Assert.True(CurrentPermissions.Allows(new Permission($"{TestAssertSpecBase}.Any")));
            Assert.True(CurrentPermissions.Allows(new Permission($"{TestAssertSpecBase}.Next")));
        });
    }

    [Fact]
    public void TestToucan_NotAllowed()
    {
        Assert.Multiple(() =>
        {
            Assert.False(CurrentPermissions.Allows(new Permission(TestRaw.For(("app", "test")))));
            Assert.False(CurrentPermissions.Allows(new Permission(TestRaw.For())));
        });
    }

    [Fact]
    public void Test_NotAllowed()
    {
        Assert.Multiple(() =>
        {
            Assert.False(CurrentPermissions.Allows(new Permission("Test")));
            Assert.False(CurrentPermissions.Allows(new Permission("Test.Assert")));
        });
    }

    [Fact]
    public void TestApp_NotAllowed()
    {
        Assert.Multiple(() =>
        {
            Assert.False(CurrentPermissions.Allows(TestAssertApp.For(("app", "Test"))));
            Assert.False(CurrentPermissions.Allows(TestAssertApp.For(("app", "TEST"))));
            Assert.False(CurrentPermissions.Allows(TestAssertApp.For(("app", "test"))));
        });
    }

    [Fact]
    public void TestApp_Allowed()
    {
        Assert.Multiple(() =>
        {
            Assert.True(CurrentPermissions.Allows(TestAssertApp.For(("app", "MyApp"))));
            Assert.True(CurrentPermissions.Allows(TestAssertApp.For(("app", "MYAPP"))));
            Assert.True(CurrentPermissions.Allows(TestAssertApp.For(("app", "myapp"))));
        });
    }

    [Fact]
    public void TestApp1_NotAllowed()
    {
        Assert.Multiple(() =>
        {
            Assert.False(CurrentPermissions.Allows(TestAssertApp.For(("app", "MyApp1"))));
            Assert.False(CurrentPermissions.Allows(TestAssertApp.For(("app", "MYAPP1"))));
            Assert.False(CurrentPermissions.Allows(TestAssertApp.For(("app", "myapp1"))));
            Assert.False(CurrentPermissions.Allows(TestAssertApp.For(("app", "MyApp2"))));
            Assert.False(CurrentPermissions.Allows(TestAssertApp.For(("app", "MYAPP2"))));
            Assert.False(CurrentPermissions.Allows(TestAssertApp.For(("app", "myapp2"))));
        });
    }

    [Fact]
    public void TestAppSchema_Allowed()
    {
        Assert.Multiple(() =>
        {
            Assert.True(CurrentPermissions.Allows(TestAssertAppSchema.For(("app", "MyApp1"), ("schema", "MySchema1"))));
            Assert.True(CurrentPermissions.Allows(TestAssertAppSchema.For(("app", "MyApp1"), ("schema", "MySchema2"))));
            Assert.True(CurrentPermissions.Allows(TestAssertAppSchema.For(("app", "MyApp2"), ("schema", "MySchema1"))));
            Assert.True(CurrentPermissions.Allows(TestAssertAppSchema.For(("app", "MyApp2"), ("schema", "MySchema2"))));
        });
    }

    [Fact]
    public void TestAppSchemaOther_NotAllowed()
    {
        Assert.Multiple(() =>
        {
            Assert.False(CurrentPermissions.Allows(TestAssertAppSchema.For(("app", "MyApp1"), ("schema", "MySchemaOther"))));
            Assert.False(CurrentPermissions.Allows(TestAssertAppSchema.For(("app", "MyApp2"), ("schema", "MySchemaOther"))));
        });
    }

    [Fact]
    public void TestSectionSchema_NotAllowed()
    {
        Assert.Multiple(() =>
        {
            Assert.False(CurrentPermissions.Allows(TestAssertAppSchema.For(("app", "Section"), ("schema", "MySchema1"))));
            Assert.False(CurrentPermissions.Allows(TestAssertAppSchema.For(("app", "Section"), ("schema", "MySchema2"))));
        });
    }

    [Fact]
    public void TestSectionSchema_Allowed()
    {
        Assert.Multiple(() =>
        {
            Assert.True(CurrentPermissions.Allows(TestAssertAppSchema.For(("app", "Section"), ("schema", "MySchema3"))));
            Assert.True(CurrentPermissions.Allows(TestAssertAppSchema.For(("app", "Section"), ("schema", "Other"))));
        });
    }
}