using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Toucan.Sdk.Pipeline;

public interface IPipelineContext {}
public abstract record class PipelineContext : IPipelineContext
{
    private ImmutableDictionary<string, object> _store = ImmutableDictionary<string, object>.Empty;

    public IEnumerable<T> GetValues<T>()
    {
        return _store.Values.OfType<T>();
    }
    public bool TryGetValueNotNull<T>(string key, [NotNullWhen(true)] out T? storedValue)
        where T : notnull
    {
        if (TryGetValue(key, out T? stored) && stored is not null)
        {
            storedValue = stored;
            return true;
        }
        storedValue = default;
        return false;

    }
    public bool TryGetValue<T>(string key, [MaybeNull] out T storedValue)
    {
        if (_store.TryGetValue(key, out object? value) && value is T typedValue)
        {
            storedValue = typedValue;
            return true;
        }
        storedValue = default;
        return false;
    }
    public bool TrySetValue<T>(string key, T storedValue)
            where T : notnull
    {
        if (_store.ContainsKey(key))
            return false;
        _store = _store.Add(key, storedValue);
        return true;
    }
}
