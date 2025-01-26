using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;

namespace Toucan.Sdk.Api.TypeMappers;
public class WrapperObjectTypeMapper(Type mappedType, Func<TypeMapperContext, Type[], JsonSchema> schemaFactory) : ITypeMapper
{
    public Type MappedType { get; } = mappedType;
    public bool UseReference => true;

    public void GenerateSchema(JsonSchema schema, TypeMapperContext context)
    {
        if (!context.JsonSchemaResolver.HasSchema(context.Type, false))
            context.JsonSchemaResolver.AddSchema(context.Type, false, schemaFactory(context, context.Type.GetGenericArguments()));

        schema.Reference = context.JsonSchemaResolver.GetSchema(context.Type, false);
    }
}
