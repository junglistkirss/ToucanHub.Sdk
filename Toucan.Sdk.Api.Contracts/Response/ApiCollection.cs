namespace Toucan.Sdk.Api.Contracts.Response;

public record class ApiCollection<T>
{
    public long? DomainCount { get; init; }
    public int Count => Collection?.Length ?? 0;
    public T[] Collection { get; init; } = [];

}
