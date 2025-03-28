using System.Collections;
using System.Globalization;
using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.JsonData;
public sealed record class JsonDataArray : IEquatable<JsonDataArray>, IReadOnlyList<JsonDataValue>, ITarget<object?[]>
{
    private readonly JsonDataValue[] _values;

    public int Count => _values.Length;

    public JsonDataValue this[int index] { get => _values[index]; }

    public JsonDataArray()
    {
        _values = [];
    }

    //public JsonDataArray(int capacity)
    //{
    //    _values = new(capacity);
    //}

    //public RawArray(RawArray source) 

    //{
    //    _values = new(source._values);

    //}

    public JsonDataArray(IEnumerable<JsonDataValue>? source) : this()
    {
        if (source != null)
            _values = [.. source];
    }


    //public override bool Equals(object? obj) => Equals(obj as RawArray);

    //public bool Equals(RawArray? array) => ReferenceEquals(this, array) || EqualsList(ToArray(), array?.ToArray() ?? []);
    //private static bool EqualsList<T>(T[] list, T[] other) => EqualsList(list, other, EqualityComparer<T>.Default);

    //private static bool EqualsList<T>(T[] list, T[] other, EqualityComparer<T> comparer)
    //{
    //    if (other == null)
    //        return false;

    //    if (ReferenceEquals(list, other))
    //        return true;

    //    if (list.Length != other.Length)
    //        return false;

    //    for (int i = 0; i < list.Length; i++)
    //    {
    //        if (!comparer.Equals(list[i], other[i]))
    //            return false;
    //    }

    //    return true;
    //}
    public override int GetHashCode() => SequentialHashCode(this);
    private static int SequentialHashCode<T>(IEnumerable<T> collection) => SequentialHashCode(collection, EqualityComparer<T>.Default);

    private static int SequentialHashCode<T>(IEnumerable<T> collection, EqualityComparer<T> comparer)
    {
        int hashCode = 17;

        foreach (T? item in collection)
        {
            if (!Equals(item, null))
                hashCode = hashCode * 23 + comparer.GetHashCode(item);
        }

        return hashCode;
    }
    public string ToJsonFragment() => $"[{string.Join(", ", this.Select(x => x.ToJsonFragment()))}]";
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

    public bool Contains(JsonDataValue item)
    {
        return _values.Contains(item);
    }

    public void CopyTo(JsonDataValue[] array, int arrayIndex)
    {
        _values.CopyTo(array, arrayIndex);
    }

    public IEnumerator<JsonDataValue> GetEnumerator()
    {
        return _values.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    public object?[] ToTarget()
    {
        return [.. _values.Select(x => x.ToTarget())];
    }
}