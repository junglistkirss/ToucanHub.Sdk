using NSwag.Generation.AspNetCore;

namespace Toucan.Sdk.Api.Extensions;

public record class ApiConfiguration
{
    public static readonly ApiConfiguration Default = new()
    {
        OpenApiDocumentGenerator = null,
    };

    public Action<AspNetCoreOpenApiDocumentGeneratorSettings>? OpenApiDocumentGenerator { get; init; }
}
