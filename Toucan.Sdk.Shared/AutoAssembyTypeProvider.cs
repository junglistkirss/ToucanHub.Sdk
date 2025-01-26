//using System.Reflection;
//using Toucan.Sdk.Utils;

//namespace Toucan.Sdk.Contracts.Registry;


//public sealed class AutoAssembyTypeProvider<T>(TypeFinder filter) : ITypeProvider
//{
//    public void Map(TypeNameRegistry? typeNameRegistry = null)
//    {
//        typeNameRegistry ??= TypeNameRegistry.Instance;

//        _ = typeNameRegistry.MapUnmapped(typeof(T).Assembly, TypeName.GetTypeName, predicate: filter);
//    }
//}


//public sealed class AppDomainTypeProvider(TypeFinder filter, params string[] pattern) : ITypeProvider
//{
//    public void Map(TypeNameRegistry? typeNameRegistry = null)
//    {
//        typeNameRegistry ??= TypeNameRegistry.Instance;
//        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetAll(pattern)))
//        {
//            _ = typeNameRegistry.MapUnmapped(assembly, TypeName.GetTypeName, predicate: filter);
//        }
//    }
//}
