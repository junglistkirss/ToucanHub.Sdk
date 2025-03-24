using System.Collections;
using System.Diagnostics.Contracts;
using Toucan.Sdk.Contracts.Extensions;
using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.JsonData;

public sealed record class JsonDataObject : IReadOnlyDictionary<string, JsonDataValue>, IEquatable<JsonDataObject>, ITarget<IReadOnlyDictionary<string, object?>>
{
    //public static readonly JsonDataObject Dummy = new() { { "dummy", true } };


    private readonly IDictionary<string, JsonDataValue> _values;

    public ICollection<string> Keys => _values.Keys;

    public ICollection<JsonDataValue> Values => _values.Values;

    public int Count => _values.Count;

    IEnumerable<string> IReadOnlyDictionary<string, JsonDataValue>.Keys => _values.Keys;

    IEnumerable<JsonDataValue> IReadOnlyDictionary<string, JsonDataValue>.Values => _values.Values;

    //public override RawValueType Type => RawValueType.Object;

    public JsonDataValue this[string key] { get => _values[key]; }

    public JsonDataObject()
    {
        _values = new Dictionary<string, JsonDataValue>(StringComparer.OrdinalIgnoreCase);
    }

    public JsonDataObject(int capacity)
    {
        _values = new Dictionary<string, JsonDataValue>(capacity, StringComparer.OrdinalIgnoreCase);

    }

    //public RawObject(RawObject source)
    //    : base(source)
    //{
    //}

    public JsonDataObject(IEnumerable<KeyValuePair<string, JsonDataValue>> source)

    {
        _values = new Dictionary<string, JsonDataValue>(source, StringComparer.OrdinalIgnoreCase);
    }

    public JsonDataValue GetOrNull(string key)
    {
        if (TryGetValue(key, out JsonDataValue value))
            return value;
        return JsonDataValue.Null;
    }

    //public override bool Equals(object? obj) => Equals(obj as RawObject);

    public bool Equals(JsonDataObject? other) => _values.EqualsDictionary(other?._values);

    public override int GetHashCode() => _values.DictionaryHashCode(StringComparer.OrdinalIgnoreCase, EqualityComparer<JsonDataValue>.Default);

    public string ToJsonFragment() => $"{{{string.Join(", ", this.Select(x => $"\"{x.Key}\":{x.Value.ToJsonFragment()}"))}}}";

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

    //public JsonDataObject Put(string key, JsonDataValue value)
    //{
    //    this[key] = value;

    //    return this;
    //}

    //public void Add(string key, JsonDataValue value)
    //{
    //    _values.Add(key, value);
    //}

    public bool ContainsKey(string key)
    {
        return _values.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return _values.Remove(key);
    }

    public void Add(KeyValuePair<string, JsonDataValue> item)
    {
        _values.Add(item);
    }

    public void Clear()
    {
        _values.Clear();
    }

    public bool Contains(KeyValuePair<string, JsonDataValue> item)
    {
        return _values.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, JsonDataValue>[] array, int arrayIndex)
    {
        _values.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, JsonDataValue> item)
    {
        return _values.Remove(item);
    }

    public IEnumerator<KeyValuePair<string, JsonDataValue>> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    public IReadOnlyDictionary<string, object?> ToTarget()
    {
        return _values.ToDictionary(x => x.Key, x => x.Value.ToTarget());
    }

    [Pure]
    public JsonDataObject MergeInto(JsonDataObject target) => new(target.Merge(StringComparer.OrdinalIgnoreCase, this));

    //private class StringIEqualityComparerOrdinalIgnoreCase : EqualityComparer<string>
    //{
    //    public override bool Equals(string? x, string? y)
    //    {
    //        return StringComparer.OrdinalIgnoreCase.Equals(x, y);
    //    }

    //    public override int GetHashCode([DisallowNull] string obj)
    //    {
    //        return obj.GetHashCode(StringComparison.OrdinalIgnoreCase);
    //    }
    //}
}
