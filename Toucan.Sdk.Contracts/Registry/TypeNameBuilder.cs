namespace Toucan.Sdk.Contracts.Registry;

//public static class TypeNameBuilder
//{
//    public static string TypeName(this Type type, bool camelCase, params string[] suffixes)
//    {
//        string? typeName = type.Name;

//        if (suffixes != null)
//        {
//            foreach (string? suffix in suffixes)
//            {
//                if (typeName.EndsWith(suffix, StringComparison.Ordinal))
//                {
//                    typeName = typeName[..^suffix.Length];

//                    break;
//                }
//            }
//        }

//        return camelCase ? typeName.ToCamelCase() : typeName;
//    }
//}
