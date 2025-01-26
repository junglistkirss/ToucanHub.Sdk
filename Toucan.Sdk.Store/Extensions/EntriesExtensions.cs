using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Toucan.Sdk.Store.Extensions;

public static class EntriesExtensions
{
    public static IEnumerable<Change> GetChanges(this EntityEntry entry)
    {
        foreach (PropertyEntry? prop in entry.Properties)
        {
            if (prop == null)
                continue;
            if (prop.IsModified && !(prop.OriginalValue?.Equals(prop.CurrentValue) ?? false)) //Only create a log if the value changes
            {
                object? o = prop.OriginalValue;
                object? c = prop.CurrentValue;
                yield return new Change(prop.Metadata.Name, o, c);
            }
        }
    }

    //public static IEnumerable<Change> GetChanges<T>(this EntityEntry<T> entry)
    //            where T : class
    //{
    //    foreach (PropertyEntry? prop in entry.Properties)
    //    {
    //        if (prop == null)
    //            continue;
    //        if (prop.IsModified && !(prop.OriginalValue?.Equals(prop.CurrentValue) ?? false)) //Only create a log if the value changes
    //        {
    //            object? o = prop.OriginalValue;
    //            object? c = prop.CurrentValue;
    //            yield return new Change(prop.Metadata.Name, o, c);
    //        }
    //    }
    //}
}