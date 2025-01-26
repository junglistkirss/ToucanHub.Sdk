//using NJsonSchema;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Toucan.Sdk.Api.Contracts.Response.Convention;

[DataContract, Serializable]
public record class ModelConvention<T>
{
    [DataMember]
    public T Model { get; init; } = default!;

    //[DataMember]
    //internal JsonSchema? Schema => JsonSchema.FromType<T>();

    [DataMember]
    public ModelLink Self { get; init; }

    [DataMember]
    public IReadOnlyDictionary<string, ModelLink> Links { get; init; } = ImmutableDictionary<string, ModelLink>.Empty;
}

public delegate ModelConvention<T> ConventionMapper<T>(T model);
public static class ModelConventionMapper<T>
{
    public static ConventionMapper<T> Self(Func<T, ModelLink>? self = null)
    {
        return (model) => new ModelConvention<T>
        {
            Model = model,
            Self = self?.Invoke(model) ?? ModelLink.Empty,
        };
    }
}