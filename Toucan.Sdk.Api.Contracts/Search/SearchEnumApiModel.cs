namespace Toucan.Sdk.Api.Contracts.Search;

public sealed class SearchEnumApiModel<TEnum>
   where TEnum : struct, IConvertible
{
    public TEnum Value { get; set; }
    public string? Method { get; set; }
}
