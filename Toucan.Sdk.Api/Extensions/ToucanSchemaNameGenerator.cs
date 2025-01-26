using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Annotations;
using NJsonSchema.Generation;

namespace Toucan.Sdk.Api.Extensions;

public class ToucanSchemaNameGenerator : ISchemaNameGenerator
{
    /// <summary>Generates the name of the JSON Schema.</summary>
    /// <param name="type">The type.</param>
    /// <returns>The new name.</returns>
    public virtual string Generate(Type type)
    {
        var cachedType = type.ToCachedType();

        var jsonSchemaAttribute = cachedType.GetAttribute<JsonSchemaAttribute>(true);

        if (!string.IsNullOrEmpty(jsonSchemaAttribute?.Name))
        {
            return jsonSchemaAttribute!.Name!;
        }

        var nType = type.ToCachedType();

        if (nType.Type.IsConstructedGenericType)
        {
            var n = GetName(nType).Split('`').First() + "Of" +
                   string.Join("And", nType.GenericArguments
                       .Select(a => Generate(a.OriginalType)));
            return n;
        }
        if (nType.Type.IsArray)
        {
            Type? inner = nType.Type.GetElementType();
            if (inner is not null)
            {
                var n = "ListOf" + Generate(inner);
                return n;
            }
        }
        var d = GetName(nType);
        return d;
    }

    private static string GetName(CachedType cType)
    {
        return
            cType.Name == "Int16" ? GetNullableDisplayName(cType, "Short") :
            cType.Name == "Int32" ? GetNullableDisplayName(cType, "Integer") :
            cType.Name == "Int64" ? GetNullableDisplayName(cType, "Long") :
            GetNullableDisplayName(cType, cType.Name);
    }

    private static string GetNullableDisplayName(CachedType type, string actual)
    {
        return (type.IsNullableType ? "Nullable" : "") + actual;
    }
}
