namespace Toucan.Sdk.Api.Contracts.Response.Convention;

public readonly record struct ModelLink
{
    public static readonly ModelLink Empty = new();

    public string Href { get; init; }
    public string Method { get; init; }
}
