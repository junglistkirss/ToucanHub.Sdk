using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace ToucanHub.Sdk.Pipeline;

public interface IPipelineContext { }
public abstract class PipelineContext : IPipelineContext
{
    protected PipelineContext()
    {
        _store = new();
    }

    [ExcludeFromCodeCoverage]
    protected PipelineContext(int concurrencyLevel, int capacity)
    {
        _store = new(concurrencyLevel, capacity);
    }

    [ExcludeFromCodeCoverage]
    protected PipelineContext(IEnumerable<KeyValuePair<string, object?>> collection)
    {
        _store = new(collection);
    }

    [ExcludeFromCodeCoverage]
    protected PipelineContext(IEqualityComparer<string>? comparer)
    {
        _store = new(comparer);
    }

    [ExcludeFromCodeCoverage]
    protected PipelineContext(IEnumerable<KeyValuePair<string, object?>> collection, IEqualityComparer<string>? comparer)
    {
        _store = new(collection, comparer);
    }

    [ExcludeFromCodeCoverage]
    protected PipelineContext(int concurrencyLevel, IEnumerable<KeyValuePair<string, object?>> collection, IEqualityComparer<string>? comparer)
    {
        _store = new(concurrencyLevel, collection, comparer);

    }

    [ExcludeFromCodeCoverage]
    protected PipelineContext(int concurrencyLevel, int capacity, IEqualityComparer<string>? comparer)
    {
        _store = new(concurrencyLevel, capacity, comparer);

    }

    private readonly ConcurrentDictionary<string, object?> _store;

    public IEnumerable<T> GetValues<T>()
    {
        return _store.Values.OfType<T>();
    }
    public IEnumerable<KeyValuePair<string, T>> GetKeyValuePairs<T>()
    {
        foreach ((string key, object? value) in _store)
        {
            if(value is T typedValue)
                yield return new KeyValuePair<string, T>(key, typedValue);
        }
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

    #region DefaulConcurrentDictionary methods

    [ExcludeFromCodeCoverage]
    public bool TryAdd(string key, object? storedValue) => _store.TryAdd(key, storedValue);

    [ExcludeFromCodeCoverage]
    public bool TryUpdate(string key, object? newValue, object? compareValue) => _store.TryUpdate(key, newValue, compareValue);

    [ExcludeFromCodeCoverage]
    public bool TryRemove(string key, [MaybeNullWhen(false)] out object? value) => _store.TryRemove(key, out value);

    [ExcludeFromCodeCoverage]
    public object? GetOrAdd(string key, Func<string, object?> valueFactory) => _store.GetOrAdd(key, valueFactory);

    [ExcludeFromCodeCoverage]
    public object? GetOrAdd<TArg>(string key, Func<string, TArg, object?> valueFactory, TArg factoryArgument) => _store.GetOrAdd(key, valueFactory, factoryArgument);

    [ExcludeFromCodeCoverage]
    public object? GetOrAdd(string key, object? value) => _store.GetOrAdd(key, value);

    [ExcludeFromCodeCoverage]
    public object? AddOrUpdate<TArg>(string key, Func<string, TArg, object?> addValueFactory, Func<string, object?, TArg, object?> updateValueFactory, TArg factoryArgument) => _store.AddOrUpdate(key, addValueFactory, updateValueFactory, factoryArgument);

    [ExcludeFromCodeCoverage]
    public object? AddOrUpdate(string key, Func<string, object?> addValueFactory, Func<string, object?, object?> updateValueFactory) => _store.AddOrUpdate(key, addValueFactory, updateValueFactory);

    [ExcludeFromCodeCoverage]
    public object? AddOrUpdate(string key, object? addValue, Func<string, object?, object?> updateValueFactory) => _store.AddOrUpdate(key, addValue, updateValueFactory);

    [ExcludeFromCodeCoverage]
    public bool IsEmpty => _store.IsEmpty;

    [ExcludeFromCodeCoverage]
    public int Count => _store.Count;

    [ExcludeFromCodeCoverage]
    public ICollection<string> Keys => _store.Keys;

    [ExcludeFromCodeCoverage]
    public ICollection<object?> Values => _store.Values;

    [ExcludeFromCodeCoverage]
    public void Clear() => _store.Clear();
    #endregion
}
