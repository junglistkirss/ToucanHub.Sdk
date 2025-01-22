namespace Toucan.Sdk.Contracts.Query.Page;

public readonly record struct Pagination(int Limit, int Offset = 0)
{
    public static Pagination Create(int? offset = 0, int? limit = null, int defaultLimit = 100, int upLimit = 10000)
    {
        return new Pagination()
        {
            Limit = Math.Max(1, Math.Min(limit ?? defaultLimit, upLimit)),
            Offset = Math.Max(0, offset ?? 0),
        };
    }


    public static readonly Pagination None = new(0);
    public static readonly Pagination Default = new(10);
    public static readonly Pagination Infinite = new(int.MaxValue);

    public int Limit { get; init; } = Limit;
    public int Offset { get; init; } = Offset;
    public override string ToString() => $"{Offset}:{Limit}";
}
