namespace ToucanHub.Sdk.Contracts.Query.Page;

public readonly record struct SortOption<TSort>
    where TSort : notnull
{

    public static SortOption<TSort>[] Simple(TSort? sortBy = default, SortDirection? sortDirection = SortDirection.Asc)
    {
        return [new SortOption<TSort>
        {
            Field = sortBy,
            Direction = sortDirection ?? SortDirection.Asc,
        }];
    }
    public static SortOption<TSort>[] Simple(TSort? sort = default, bool? desc = false)
    {
        //_ = sort.TryConvertOrNull(out T? sorting);
        return [new SortOption<TSort>
        {
            Field = sort,
            Direction = desc ?? false ? SortDirection.Desc : SortDirection.Asc,
        }];
    }

    public static SortOption<TSort> Empty => new();

    public TSort? Field { get; init; }
    public SortDirection Direction { get; init; }
    public override string ToString() => $"{(Field is not null ? Field : "default")} {Direction}";
}
