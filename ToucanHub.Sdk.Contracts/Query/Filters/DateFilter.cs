using ToucanHub.Sdk.Contracts.Query.Filters.Abstractions;

namespace ToucanHub.Sdk.Contracts.Query.Filters;

public sealed record class DateFilter : BaseFilter<DateFilterMethod, DateTime>, IFilter<DateFilter>
{
    public static readonly DateFilter Empty = new() { Method = default, Value = default };


    public override int GetHashCode() => base.GetHashCode();
}
