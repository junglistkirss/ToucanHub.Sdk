namespace ToucanHub.Sdk.Contracts.Registry;

public delegate bool TypeFinder(Type type);
public interface ITypeProvider
{
    void Map(TypeNameRegistry typeNameRegistry);
}
