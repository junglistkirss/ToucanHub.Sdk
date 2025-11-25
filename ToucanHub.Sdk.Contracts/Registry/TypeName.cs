using System.Reflection;

namespace ToucanHub.Sdk.Contracts.Registry;

public static class TypeName
{
    public static string GetTypeNameOrFullName(this Type type)
    {
        if (type.TryGetTypeName(out string? name))
            return name;
        return type.FullName ?? throw new InvalidDataException("FullName is null");
    }

    public static string GetTypeNameOrAssemblyQualifiedName(this Type type)
    {
        if (type.TryGetTypeName(out string? name))
            return name;
        return type.AssemblyQualifiedName ?? throw new InvalidDataException("AssemblyQualifiedName is null");
    }

    public static bool TryGetTypeName(this Type type, [NotNullWhen(true)] out string? name)
    {
        name = null;
        TypeNameAttribute? typeNameAttribute = type.GetCustomAttribute<TypeNameAttribute>();
        if (!string.IsNullOrWhiteSpace(typeNameAttribute?.TypeName))
        {
            name = typeNameAttribute.TypeName;
            return true;
        }
        return false;
    }

    public static string GetTypeName(this Type type)
    {
        if (type.TryGetTypeName(out string? name))
            return name;
        return type.Name;
    }
}
