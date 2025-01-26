namespace Toucan.Sdk.Store.Extensions;

public delegate bool TryParseInlineStruct<T>(string? value, out T result)
     where T : struct;
public record Change(string? Name, object? Orginal, object? Current);

////public record ChangeRecord<T>(EventChangeType ChangeType, T? Model, IEnumerable<Change>? Changes = null, int Version = 0);
//public enum EventChangeType
//{
//    Unspecified,

//    Created,
//    Updated,
//    Deleted,
//}
//public record ChangeRecord(EventChangeType EventChange, string? TypeName, IEnumerable<Change>? Changes = null);