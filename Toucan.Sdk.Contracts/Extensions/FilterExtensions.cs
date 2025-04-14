using Toucan.Sdk.Contracts.Criterias;
using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Query.Filters;
using Toucan.Sdk.Contracts.Query.Filters.Abstractions;

namespace Toucan.Sdk.Contracts.Extensions;

public static class FilterExtensions
{
    public static bool IsDefault<T>(this EnumFilter<T>? filter) => filter?.Equals(EnumFilter<T>.Empty) ?? true;
    public static bool IsDefault(this DateFilter? filter) => filter?.Equals(DateFilter.Empty) ?? true;
    public static bool IsDefault(this StringFilter? filter) => filter?.Equals(StringFilter.None) ?? true;

    public static bool IsDefault<T>(this StringifyFilter<T>? filter) => filter?.Equals(StringFilter.None) ?? true;


    public static bool IsValid([NotNullWhen(true)] this StringFilter? filter)
    {
        return filter is not null
            && (
                (!string.IsNullOrWhiteSpace(filter.Value) && filter.Method != StringFilterMethod.IsNull)
                || (filter.Method == StringFilterMethod.IsNull)
            );
    }

    public static bool IsValidVersionCriteria([NotNullWhen(true)] this IVersionCriteria? filter)
    {
        if (filter is null)
            return false;

        return filter.Version.IsValid();
    }

    public static bool IsValidEntityCriteria([NotNullWhen(true)] this IEntityCriteria? filter)
    {
        if(filter is null)
            return false;

        return filter.Modifier.HasValue || filter.Creator.HasValue || filter.Created.IsValid() || filter.LastModified.IsValid();
    }
    public static bool IsValid([NotNullWhen(true)] this StringifyFilter<Slug>? filter)
    {
        return filter is not null
            && (
                (!filter.Value.IsEmpty() && filter.Method != StringFilterMethod.IsNull)
                || (filter.Method == StringFilterMethod.IsNull)
            );
    }

    public static bool IsValid([NotNullWhen(true)] this StringifyFilter<DomainId>? filter)
    {
        return filter is not null
            && (
                (filter.Value != DomainId.Empty && filter.Method != StringFilterMethod.IsNull)
                || (filter.Method == StringFilterMethod.IsNull)
            );
    }


    public static bool IsValid([NotNullWhen(true)] this StringifyFilter<RefToken>? filter)
    {
        return filter is not null
            && (
                (!filter.Value.IsEmpty && filter.Method != StringFilterMethod.IsNull)
                || (filter.Method == StringFilterMethod.IsNull)
            );
    }


    public static bool IsValid([NotNullWhen(true)] this DateFilter? filter)
    {
        return filter is not null
            && (
                (filter.Value >= DateTime.MinValue && filter.Value <= DateTime.MaxValue && filter.Method != DateFilterMethod.IsNull)
                || (filter.Method == DateFilterMethod.IsNull)
            );
    }

    public static bool IsValid([NotNullWhen(true)] this NumericFilter<double>? filter)
    {
        return filter is not null
            && filter.Value != double.NaN;
    }

    public static bool IsValid([NotNullWhen(true)] this NumericFilter<long>? filter)
    {
        return filter is not null
            && filter.Value >= long.MinValue
            && filter.Value <= long.MaxValue;
    }

    public static bool IsValid([NotNullWhen(true)] this ExistsFilter<DomainId>? filter) => filter.IsValid<DomainId>();
    public static bool IsValid([NotNullWhen(true)] this ExistsFilter<Slug>? filter) => filter.IsValid<Slug>();
    public static bool IsValid([NotNullWhen(true)] this ExistsFilter<Tag>? filter) => filter.IsValid<Tag>();
    public static bool IsValid<T>([NotNullWhen(true)] this ExistsFilter<T>? filter)
    {
        return filter is not null
            && filter.Value?.Length > 0;
    }
}
