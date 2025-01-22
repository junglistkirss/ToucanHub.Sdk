using Toucan.Sdk.Contracts.Query.Filters;

namespace Toucan.Sdk.Contracts.Extensions;

public static class FilterExtensions
{
    public static bool IsDefault<T>(this EnumFilter<T>? filter) => filter?.Equals(EnumFilter<T>.Empty) ?? true;
    public static bool IsDefault(this DateFilter? filter) => filter?.Equals(DateFilter.Empty) ?? true;
    public static bool IsDefault(this StringFilter? filter) => filter?.Equals(StringFilter.None) ?? true;

    public static bool IsDefault<T>(this StringifyFilter<T>? filter) => filter?.Equals(StringFilter.None) ?? true;

}
