using ToucanHub.Sdk.Contracts.Query.Page;

namespace ToucanHub.Sdk.Contracts.Query;

public interface IPaginated<TSort>
    where TSort : notnull
{
    Pagination Pagination { get; }
    SortOption<TSort>[] SortOptions { get; }
}
