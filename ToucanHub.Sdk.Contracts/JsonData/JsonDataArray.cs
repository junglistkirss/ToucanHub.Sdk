using System.Collections;
using System.Globalization;

namespace ToucanHub.Sdk.Contracts.JsonData;
public sealed class JsonDataArray : IEquatable<JsonDataArray>, IReadOnlyList<JsonDataValue>
{
    private readonly JsonDataValue[] _values;

    public int Count => _values.Length;

    public JsonDataValue this[int index] { get => _values[index]; }

    public JsonDataArray()
    {
        _values = [];
    }

    public JsonDataArray(IEnumerable<JsonDataValue>? source) : this()
    {
        if (source != null)
            _values = [.. source];
    }

    internal string ToJsonFragment() => $"[{string.Join(", ", this.Select(x => x.ToJsonFragment()))}]";
    public override string ToString() => ToJsonFragment();

    public bool TryGetValue(string pathSegment, [MaybeNullWhen(false)] out JsonDataValue result)
    {
        ArgumentNullException.ThrowIfNull(pathSegment);

        result = default;

        if (pathSegment != null && int.TryParse(pathSegment, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index) && index >= 0 && index < Count)
        {
            result = this[index];

            return true;
        }

        return false;
    }

    public IEnumerator<JsonDataValue> GetEnumerator()
    {
        return _values.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    public bool Equals(JsonDataArray? other)
    {
        if (other is null)
            return false;
        return _values.SequenceEqual(other._values);
    }
    public override bool Equals(object? obj)
    {
        return Equals(obj as JsonDataArray);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        foreach (JsonDataValue value in _values)
            hash.Add(value);
        return hash.ToHashCode();
    }
}