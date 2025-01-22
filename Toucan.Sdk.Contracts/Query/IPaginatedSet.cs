using Toucan.Sdk.Contracts.Query.Page;

namespace Toucan.Sdk.Contracts.Query;

public interface IPaginatedSet<TSort>
    where TSort : notnull
{
    Pagination Pagination { get; }
    SortOption<TSort>[] SortOptions { get; }
}
