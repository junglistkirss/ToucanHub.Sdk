namespace ToucanHub.Sdk.Contracts.Query.Page;

public readonly record struct Pagination
{
    public static Pagination Create(int? offset = 0, int? limit = null, int defaultLimit = 100, int upLimit = 10000)
    {
        return new Pagination(
            limit: Math.Max(1, Math.Min(limit ?? defaultLimit, upLimit)),
            offset: Math.Max(0, offset ?? 0)
        );
    }


    public static readonly Pagination None = new(0, 0);
    public static readonly Pagination Default = new(10, 0);
    public static readonly Pagination Infinite = new(-1);
    private Pagination(int value)
    {
        Limit = value;
        Offset = value;
    }
    public Pagination(int limit, int offset = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(limit);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        Limit = limit;
        Offset = offset;
    }

    public int Limit { get; }
    public int Offset { get; }
    public override string ToString() => $"[{Offset}..{Offset + Limit}]";
}
