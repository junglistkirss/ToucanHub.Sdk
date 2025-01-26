namespace Toucan.Sdk.Api.Contracts.Search;

public sealed class SearchNumericApiModel<TNumeric>
    where TNumeric : unmanaged, IComparable, IComparable<TNumeric>, IConvertible, IEquatable<TNumeric>, IFormattable
{
    public TNumeric Value { get; set; }
    public string? Method { get; set; }
}
