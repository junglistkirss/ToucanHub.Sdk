using System.Collections;

namespace Toucan.Sdk.Contracts.JsonData;

public sealed record class JsonDataObject : IReadOnlyDictionary<string, JsonDataValue>, IEquatable<JsonDataObject>
{
    private readonly IDictionary<string, JsonDataValue> _values;

    public ICollection<string> Keys => _values.Keys;

    public ICollection<JsonDataValue> Values => _values.Values;

    public int Count => _values.Count;

    IEnumerable<string> IReadOnlyDictionary<string, JsonDataValue>.Keys => _values.Keys;

    IEnumerable<JsonDataValue> IReadOnlyDictionary<string, JsonDataValue>.Values => _values.Values;

    public JsonDataValue this[string key] { get => _values[key]; }

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

    public bool TryGetValue(string pathSegment, [MaybeNullWhen(false)] out JsonDataValue result)
    {
        result = JsonDataValue.Null;
        if (!string.IsNullOrEmpty(pathSegment) && _values.TryGetValue(pathSegment, out JsonDataValue value))
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
}
