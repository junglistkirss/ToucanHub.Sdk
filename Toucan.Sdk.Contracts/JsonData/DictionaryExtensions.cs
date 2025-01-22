namespace Toucan.Sdk.Contracts.JsonData;

internal static class DictionaryExtensions
{

    public static bool EqualsReadOnlyDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, IReadOnlyDictionary<TKey, TValue>? other)
        where TKey : notnull
        => EqualsReadOnlyDictionary(dictionary, other, EqualityComparer<TValue>.Default);
    public static bool EqualsDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue>? other)
        where TKey : notnull
        => EqualsDictionary(dictionary, other, EqualityComparer<TValue>.Default);

    private static bool EqualsKeyValuePairs<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary, IEnumerable<KeyValuePair<TKey, TValue>>? other, IEqualityComparer<TValue> valueComparer)
      where TKey : notnull
    {

        if (other == null)
            return false;

        if (ReferenceEquals(dictionary, other))
            return true;

        if (dictionary is SortedDictionary<TKey, TValue> sorted && other is SortedDictionary<TKey, TValue> otherSorted)
            return sorted.SequenceEqual(otherSorted);

        return dictionary.All(x => other.Any(y => y.Key.Equals(x.Key) && valueComparer.Equals(x.Value, y.Value)));
    }

    public static bool EqualsReadOnlyDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, IReadOnlyDictionary<TKey, TValue>? other, IEqualityComparer<TValue> valueComparer)
        where TKey : notnull
        => EqualsKeyValuePairs(dictionary, other, valueComparer);
    public static bool EqualsDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue>? other, IEqualityComparer<TValue> valueComparer)
        where TKey : notnull
        => EqualsKeyValuePairs(dictionary, other, valueComparer);

    public static int DictionaryHashCode<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        where TKey : notnull
    {
        int hashCode = 17;

        foreach ((TKey key, TValue value) in dictionary.OrderBy(x => x.Key))
        {
            hashCode = hashCode * 23 + keyComparer.GetHashCode(key);

            if (!Equals(value, null))
                hashCode = hashCode * 23 + valueComparer.GetHashCode(value);
        }

        return hashCode;
    }

    public static IEnumerable<KeyValuePair<TKey, TValue>> Merge<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> target, IEqualityComparer<TKey> comparer, params IEnumerable<KeyValuePair<TKey, TValue>>[] sources)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(sources);

        Dictionary<TKey, TValue> result = new(target, comparer);

        if (sources.Length > 0)
            foreach (IEnumerable<KeyValuePair<TKey, TValue>> source in sources)
                foreach ((TKey name, TValue data) in source)
                    result.TryAdd(name, data);

        return result;
    }


}
