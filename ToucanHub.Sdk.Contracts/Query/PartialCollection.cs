using System.Text.Json.Serialization;

namespace ToucanHub.Sdk.Contracts.Query;

[method: JsonConstructor]
public sealed class PartialCollection<T>(long domainCount, IEnumerable<T> items) : List<T>(items)
{
    public static PartialCollection<T> Empty = new(0, Array.Empty<T>());

    public long DomainCount { get; set; } = domainCount;

    public new PartialCollection<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => new(DomainCount, base.ConvertAll(converter));

}
