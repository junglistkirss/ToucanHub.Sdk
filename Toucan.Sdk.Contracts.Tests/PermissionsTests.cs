using Toucan.Sdk.Contracts.Security;

namespace Toucan.Sdk.Contracts.Tests;
public class PermissionsTests
{
    //private const string TestAssertAny = "Test.Assert.Any.*";

    [Fact]
    public void TestSimple_Allowed()
    {
        PermissionSet current = new(
            "Test.Assert.Simple"
        );

        Assert.Multiple(() =>
        {
            Assert.True(current.Allows("Test.Assert.Simple"));
            Assert.True(current.Allows("Test.Assert.Simple".ToLower()));
            Assert.True(current.Allows("Test.Assert.Simple".ToUpper()));
        });
    }

    [Fact]
    public void TestChild_Allowed()
    {
        PermissionSet current = new(
            "Test.Assert.Spec.Child"
        );
        Assert.Multiple(() =>
        {
            Assert.True(current.Allows("Test.Assert.Spec.Child.Next"));
            Assert.True(current.Allows("Test.Assert.Spec.Child.Next.Sub"));
        });
    }

    [Fact]
    public void TestAny_Allowed()
    {
        PermissionSet current = new(
            "Test.Assert.Any.read"
        );
        Assert.Multiple(() =>
        {
            Assert.True(current.Allows("Test.Assert.Any.read"));
            Assert.True(current.Allows("Test.Assert.Any.read.Sub.Inner"));
        });
    }

    [Fact]
    public void TestExclusion_Allowed()
    {
        PermissionSet current = new(
            "Test.Assert.Any.write.^Sub"
        );
        Assert.Multiple(() =>
        {
            Assert.False(current.Allows("Test.Assert.Any.write.Sub"));
            Assert.False(current.Allows("Test.Assert.Any.write.Sub.next"));
            Assert.True(current.Allows("Test.Assert.Any.write.OtherSub"));
            Assert.True(current.Allows("Test.Assert.Any.write.OtherSub.next"));
        });
    }

    [Fact]
    public void TestExclusionAny_Allowed()
    {
        PermissionSet current = new(
            "Test.Assert.Any.write.^Sub.*"
        );
        Assert.Multiple(() =>
        {
            Assert.False(current.Allows("Test.Assert.Any.write.Sub"));
            Assert.False(current.Allows("Test.Assert.Any.write.Sub.next"));
            Assert.False(current.Allows("Test.Assert.Any.write.OtherSub"));
            Assert.True(current.Allows("Test.Assert.Any.write.OtherSub.next"));
        });
    }

    [Fact]
    public void TestAny_NotAllowed()
    {
        PermissionSet current = new(
            "Test.Assert.Any.read.*"
        );
        Assert.Multiple(() =>
        {
            Assert.False(current.Allows("Test.Assert.Any.read"));
            Assert.False(current.Allows("Test.Assert.Any.write.Sub"));
            Assert.False(current.Allows("Test.Assert.Any.write.Sub.next"));
        });
    }


    [Fact]
    public void TestOther_NotAllowed()
    {
        PermissionSet current = new(
           "Test.Assert.Spec"
       );
        Assert.Multiple(() =>
        {
            Assert.False(current.Allows($"PrevTest.Assert.Spec"));
            Assert.False(current.Allows($"Test.Assert.SpecNext"));
            Assert.True(current.Allows("Test.Assert.Spec.Any"));
            Assert.True(current.Allows("Test.Assert.Spec.Next"));
        });
    }

    [Fact]
    public void TestToucan_NotAllowed()
    {
        const string TestRaw = "Toucan.Test.Assert.{app}.raw.{schema}.can";

        PermissionSet current = new(
            TestRaw
        );
        Assert.Multiple(() =>
        {
            Assert.False(current.Allows(TestRaw.For(("app", "test"))));
            Assert.False(current.Allows(TestRaw.For()));
        });
    }

    [Fact]
    public void Test_NotAllowed()
    {
        PermissionSet current = new(
            "Test.Assert.Any.*"
        );
        Assert.Multiple(() =>
        {
            Assert.False(current.Allows("Test"));
            Assert.False(current.Allows("Test.Assert"));
        });
    }

    [Fact]
    public void TestApp_NotAllowed()
    {
        const string TestAssertApp = "Test.Assert.{app}";
        PermissionSet current = new(
            TestAssertApp.For(("app", "MyApp"))
        );

        Assert.Multiple(() =>
        {
            Assert.False(current.Allows(TestAssertApp.For(("app", "Test"))));
            Assert.False(current.Allows(TestAssertApp.For(("app", "TEST"))));
            Assert.False(current.Allows(TestAssertApp.For(("app", "test"))));
        });

        Assert.Multiple(() =>
        {
            Assert.True(current.Allows(TestAssertApp.For(("app", "MyApp"))));
            Assert.True(current.Allows(TestAssertApp.For(("app", "MYAPP"))));
            Assert.True(current.Allows(TestAssertApp.For(("app", "myapp"))));
        });

        Assert.Multiple(() =>
        {
            Assert.False(current.Allows(TestAssertApp.For(("app", "MyApp1"))));
            Assert.False(current.Allows(TestAssertApp.For(("app", "MYAPP1"))));
            Assert.False(current.Allows(TestAssertApp.For(("app", "myapp1"))));
            Assert.False(current.Allows(TestAssertApp.For(("app", "MyApp2"))));
            Assert.False(current.Allows(TestAssertApp.For(("app", "MYAPP2"))));
            Assert.False(current.Allows(TestAssertApp.For(("app", "myapp2"))));
        });
    }

    [Fact]
    public void TestAppSchema_Allowed()
    {
        const string TestAssertAppSchema = "Test.Assert.{app}.Inner.{schema}";
        PermissionSet current = new(
            TestAssertAppSchema.For(("app", "MyApp1|MyApp2"), ("schema", "MySchema1|MySchema2")),
            TestAssertAppSchema.For(("app", "Section"), ("schema", "^MySchema1|MySchema2"))
        );

        Assert.Multiple(() =>
        {
            Assert.True(current.Allows(TestAssertAppSchema.For(("app", "MyApp1"), ("schema", "MySchema1"))));
            Assert.True(current.Allows(TestAssertAppSchema.For(("app", "MyApp1"), ("schema", "MySchema2"))));
            Assert.True(current.Allows(TestAssertAppSchema.For(("app", "MyApp2"), ("schema", "MySchema1"))));
            Assert.True(current.Allows(TestAssertAppSchema.For(("app", "MyApp2"), ("schema", "MySchema2"))));
        });

        Assert.Multiple(() =>
        {
            Assert.False(current.Allows(TestAssertAppSchema.For(("app", "MyApp1"), ("schema", "MySchemaOther"))));
            Assert.False(current.Allows(TestAssertAppSchema.For(("app", "MyApp2"), ("schema", "MySchemaOther"))));
        });

        Assert.Multiple(() =>
        {
            Assert.False(current.Allows(TestAssertAppSchema.For(("app", "Section"), ("schema", "MySchema1"))));
            Assert.False(current.Allows(TestAssertAppSchema.For(("app", "Section"), ("schema", "MySchema2"))));
        });

        Assert.Multiple(() =>
        {
            Assert.True(current.Allows(TestAssertAppSchema.For(("app", "Section"), ("schema", "MySchema3"))));
            Assert.True(current.Allows(TestAssertAppSchema.For(("app", "Section"), ("schema", "Other"))));
        });
    }
}