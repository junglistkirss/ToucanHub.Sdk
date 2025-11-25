using System.Collections;
using System.Diagnostics.Contracts;

namespace ToucanHub.Sdk.Contracts.JsonData;

public sealed class JsonDataObject : IReadOnlyDictionary<string, JsonDataValue>, IEquatable<JsonDataObject>
{
    private readonly Dictionary<string, JsonDataValue> _values;

    public ICollection<string> Keys => _values.Keys;

    public ICollection<JsonDataValue> Values => _values.Values;

    public int Count => _values.Count;

    IEnumerable<string> IReadOnlyDictionary<string, JsonDataValue>.Keys => _values.Keys;

    IEnumerable<JsonDataValue> IReadOnlyDictionary<string, JsonDataValue>.Values => _values.Values;

    public JsonDataValue this[string key]
    {
        get
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            return _values[key];
        }
    }

    public JsonDataObject()
    {
        _values = new Dictionary<string, JsonDataValue>(StringComparer.OrdinalIgnoreCase);
    }
    public JsonDataObject(int capacity)
    {
        _values = new Dictionary<string, JsonDataValue>(capacity, StringComparer.OrdinalIgnoreCase);
    }

    public JsonDataObject(IEnumerable<KeyValuePair<string, JsonDataValue>> source)
    {
        _values = new Dictionary<string, JsonDataValue>(source, StringComparer.OrdinalIgnoreCase);
    }

    internal string ToJsonFragment() => $"{{{string.Join(", ", this.Select(x => $"\"{x.Key}\":{x.Value.ToJsonFragment()}"))}}}";

    public override string ToString() => ToJsonFragment();

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out JsonDataValue result)
    {
        result = JsonDataValue.Null;
        if (!string.IsNullOrEmpty(key) && _values.TryGetValue(key, out JsonDataValue value))
        {
            result = value;
            return true;
        }
        return false;

    }
    public bool ContainsKey(string key)
    {
        return _values.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<string, JsonDataValue>> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    public bool Equals(JsonDataObject? other)
    {
        if (other is null)
            return false;
        return _values.SequenceEqual(other._values);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as JsonDataObject);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        foreach ((string key, JsonDataValue value) in _values.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
        {
            hash.Add(key, StringComparer.OrdinalIgnoreCase);
            hash.Add(value);
        }
        return hash.ToHashCode();
    }

    /// <summary>
    /// This method will create a new <see cref="JsonDataObject"/>
    /// <para>
    /// Properties of <paramref name="other"/> will replace exiting properties in current <see cref="JsonDataObject"/> instance
    /// </para>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    [Pure]
    public JsonDataObject Merge(IEnumerable<KeyValuePair<string, JsonDataValue>> other)
    {
        Dictionary<string, JsonDataValue> copy = new(other);
        foreach ((string key, JsonDataValue value) in _values)
        {
            copy.TryAdd(key, value);
        }
        return new(copy);
    }
}
