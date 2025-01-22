namespace Toucan.Sdk.Contracts.Registry;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public class TypeNameAttribute(string typeName) : Attribute
{
    public string TypeName { get; } = typeName;
}
