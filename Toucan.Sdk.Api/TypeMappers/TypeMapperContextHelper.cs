using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;

namespace Toucan.Sdk.Api.TypeMappers;

internal static class TypeMapperContextHelper
{
    internal static JsonSchema ResolveOrGenerate(this TypeMapperContext context, Type type)
    {
        if (context.JsonSchemaResolver.HasSchema(type, type.IsEnum))
            return context.JsonSchemaResolver.GetSchema(type, type.IsEnum);
        else
            return context.JsonSchemaGenerator.Generate(type, context.JsonSchemaResolver);
    }
}
