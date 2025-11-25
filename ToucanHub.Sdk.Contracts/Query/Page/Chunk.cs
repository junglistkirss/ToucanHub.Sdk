namespace ToucanHub.Sdk.Contracts.Query.Page;

public readonly record struct Chunk<TSort>
        where TSort : notnull
{
    public static readonly Chunk<TSort> Default = new()
    {
        Pagination = Pagination.Default,
        SortOptions = []
    };


    public static Chunk<TSort> Create(int? offset = null, int? limit = null, TSort? sort = default, bool desc = false)
    {

        return new Chunk<TSort>()
        {
            Pagination = Pagination.Create(offset, limit),
            SortOptions = [.. SortOption<TSort>.Simple(sort, desc)]
        };
    }
    public static Chunk<TSort> Create(int? offset = null, int? limit = null, TSort? sort = default, SortDirection desc = SortDirection.Asc)
    {

        return new Chunk<TSort>()
        {
            Pagination = Pagination.Create(offset, limit),
            SortOptions = [.. SortOption<TSort>.Simple(sort, desc)]
        };
    }

    public Pagination Pagination { get; init; }
    public SortOption<TSort>[] SortOptions { get; init; }

}
