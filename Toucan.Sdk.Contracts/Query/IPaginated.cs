using Toucan.Sdk.Contracts.Query.Page;

namespace Toucan.Sdk.Contracts.Query;

public interface IPaginated<TSort>
    where TSort : notnull
{
    Pagination Pagination { get; }
    SortOption<TSort>[] SortOptions { get; }
}
