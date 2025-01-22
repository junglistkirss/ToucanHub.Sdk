using System.Reflection;

namespace Toucan.Sdk.Utils;

public static class ReflectionExtensions
{

    public static PropertyInfo[] GetPublicProperties(this Type type)
    {
        const BindingFlags bindingFlags =
            BindingFlags.FlattenHierarchy |
            BindingFlags.Public |
            BindingFlags.Instance;

        if (!type.IsInterface)
            return type.GetProperties(bindingFlags);

        HashSet<PropertyInfo> flattenProperties = [];

        List<Type>? considered =
        [
            type
        ];

        Queue<Type>? queue = new();

        queue.Enqueue(type);

        while (queue.Count > 0)
        {
            Type? subType = queue.Dequeue();

            foreach (Type? subInterface in subType.GetInterfaces())
            {
                if (considered.Contains(subInterface))
                    continue;

                considered.Add(subInterface);

                queue.Enqueue(subInterface);
            }

            PropertyInfo[]? typeProperties = subType.GetProperties(bindingFlags);

            foreach (PropertyInfo? property in typeProperties)
            {
                if (property != null)
                    _ = flattenProperties.Add(property);
            }
        }

        return [.. flattenProperties];
    }

    public static bool Implements<T>(this Type type) => type.Implements(typeof(T));

    public static bool Implements(this Type type, Type interfaceType)
    {
        return type.GetInterfaces().Any(x =>
        {
            if (interfaceType.IsGenericTypeDefinition && x.IsGenericType)
                return x.GetGenericTypeDefinition() == interfaceType;
            return x == interfaceType;
        });
    }

    public static IEnumerable<Type> FindAll<T>(this IEnumerable<Assembly> assemblies)
    {
        foreach (Assembly a in assemblies)
        {
            foreach (Type t in a.FindAll<T>())
                yield return t;
        }
    }

    public static IEnumerable<Type> FindAll<T>(this Assembly assembly)
    {
        foreach (Type item in assembly.GetTypes())
        {
            if (item.IsClass && !item.IsAbstract && !item.IsGenericType)
            {
                if (item.IsAssignableTo(typeof(T)))
                    yield return item;
            }

        }
    }

    public static IEnumerable<T> ResolveAll<T>(this IEnumerable<Assembly> assemblies)
    {
        foreach (Assembly a in assemblies)
        {
            foreach (T t in a.ResolveAll<T>())
                yield return t;
        }
    }

    public static IEnumerable<T> ResolveAll<T>(this Assembly assembly)
    {
        foreach (Type item in assembly.GetTypes())
        {
            if (item.IsClass && !item.IsAbstract && !item.IsGenericType)
            {
                if (item.IsAssignableTo(typeof(T)))
                    yield return (T)Activator.CreateInstance(item)!;
            }

        }
    }
    public static IEnumerable<Assembly> GetAll(this Assembly rootAssembly, params string[] pattern)
    {
        foreach (AssemblyName item in rootAssembly.GetReferencedAssemblies())
        {
            if (pattern.Any(item.FullName.Contains))
            {
                Assembly ass = Assembly.Load(item);
                yield return ass;
                foreach (Assembly sub in ass.GetAll())
                {
                    yield return sub;
                }
            }

        }
    }

}
