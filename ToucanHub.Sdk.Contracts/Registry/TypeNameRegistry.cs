using System.Reflection;

namespace ToucanHub.Sdk.Contracts.Registry;

public interface ITypeNameRegistry
{
    Type? GetTypeOrNull(string name);
    Type RequireType(string name);

    public string? GetNameOrNull<T>() => GetNameOrNull(typeof(T));
    string? GetNameOrNull(Type instance);

    public string RequireName<T>() => RequireName(typeof(T));
    string RequireName(Type instance);

    public string GetNameOrDefault<T>() => GetNameOrDefault(typeof(T));
    string GetNameOrDefault(Type instance);

    public string GetNameOrDefaultFullName<T>() => GetNameOrDefaultFullName(typeof(T));
    string GetNameOrDefaultFullName(Type instance);

    public string GetTypeNameOrAssemblyQualifiedName<T>() => GetTypeNameOrAssemblyQualifiedName(typeof(T));
    string GetTypeNameOrAssemblyQualifiedName(Type instance);
}

public sealed class TypeNameRegistry : ITypeNameRegistry
{
    public static readonly TypeNameRegistry Instance = new();

    private readonly Dictionary<Type, string> namesByType = [];
    private readonly Dictionary<string, Type> typesByName = new(StringComparer.OrdinalIgnoreCase);

    public TypeNameRegistry(params ITypeProvider[] providers)
    {
        if (providers is not null)
        {
            foreach (ITypeProvider provider in providers)
            {
                _ = Map(provider);
            }
        }
    }

    public TypeNameRegistry Map(ITypeProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        provider.Map(this);

        return this;
    }

    public void Map<T>(string name) => Map(typeof(T), name);
    public void Map(Type type, string name)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(name);

        lock (namesByType)
        {
            if (namesByType.TryGetValue(type, out string? existingName) && existingName != name)
            {
                string? message = $"The type '{type}' is already registered with name '{namesByType[type]}'";

                throw new ArgumentException(message, nameof(type));
            }

            namesByType[type] = name;

            if (typesByName.TryGetValue(name, out Type? existingType) && existingType != type)
            {
                string? message = $"The name '{name}' is already registered with type '{typesByName[name]}'";

                throw new ArgumentException(message, nameof(type));
            }

            typesByName[name] = type;
        }
    }

    public TypeNameRegistry MapUnmapped(Assembly assembly, Func<Type, string> mapper, TypeFinder? predicate = null)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(mapper);

        IEnumerable<Type> types = assembly.GetTypes();
        if (predicate is not null)
            types = types.Where(x => predicate(x));
        foreach (Type? type in types)
        {
            if (!namesByType.ContainsKey(type))
            {
                string typeName = mapper(type);
                ArgumentException.ThrowIfNullOrEmpty(typeName);
                Map(type, typeName);
            }
        }

        return this;
    }




    public string? GetNameOrNull(Type type)
    {
        string? result = namesByType.TryGetValue(type, out string? value) ? value : null;

        return result;
    }
    public string GetNameOrDefault(Type type)
    {
        string? result = namesByType.TryGetValue(type, out string? value) ? value : null;

        return result ?? type.GetTypeName();
    }

    public string GetNameOrDefaultFullName(Type type)
    {
        string? result = namesByType.TryGetValue(type, out string? value) ? value : null;

        return result ?? type.GetTypeNameOrFullName();
    }

    public string GetTypeNameOrAssemblyQualifiedName(Type type)
    {
        string? result = namesByType.TryGetValue(type, out string? value) ? value : null;

        return result ?? type.GetTypeNameOrAssemblyQualifiedName();
    }
    public Type? GetTypeOrNull(string name) => typesByName.GetValueOrDefault(name);

    public string RequireName(Type type)
    {
        string? result = namesByType.GetValueOrDefault(type);
        if (string.IsNullOrWhiteSpace(result) && type.IsGenericType)
        {
            IEnumerable<string> subTypes = type.GetGenericArguments().Select(RequireName);
            string gName = RequireName(type.GetGenericTypeDefinition());
            result = string.Format("{0}<{1}>", gName, string.Join(',', subTypes));
        }
        return result ?? throw new TypeNameNotFoundException($"There is no name for type '{type}");
    }

    public Type RequireType(string name)
    {
        Type? result = typesByName.GetValueOrDefault(name) ?? throw new TypeNameNotFoundException($"There is no type for name '{name}");
        return result;
    }

}
