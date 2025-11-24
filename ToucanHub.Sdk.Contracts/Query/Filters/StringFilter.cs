using ToucanHub.Sdk.Contracts.Query.Filters.Abstractions;

namespace ToucanHub.Sdk.Contracts.Query.Filters;

public sealed record class StringFilter : BaseFilter<StringFilterMethod, string?>, IFilter<StringFilter>
{
    public static readonly StringFilter None = new() { Method = default, Value = null };
    public static StringFilter IsNull() => new() { Method = StringFilterMethod.IsNull, Value = null };
    public static StringFilter IsEmpty() => new() { Method = StringFilterMethod.IsEmpty, Value = null };
    public static StringFilter Equals(string? pattern) => new() { Method = StringFilterMethod.Equals, Value = pattern };
    public static StringFilter NotEquals(string? pattern) => new() { Method = StringFilterMethod.Equals, Value = pattern };
    public static StringFilter Contains(string? pattern) => new() { Method = StringFilterMethod.Contains, Value = pattern };
    public static StringFilter StartsWith(string? pattern) => new() { Method = StringFilterMethod.StartsWith, Value = pattern };
    public static StringFilter EndsWith(string? pattern) => new() { Method = StringFilterMethod.EndsWith, Value = pattern };
}
