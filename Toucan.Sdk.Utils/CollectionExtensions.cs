namespace Toucan.Sdk.Utils;
public static class CollectionExtensions
{

    public static IEnumerable<T> OmitNull<T>(this IEnumerable<T?> query)
         where T : class => query.Where(x => x != null)!;

    public static IEnumerable<string> OmitNullOrEmpty(this IEnumerable<string?> query) => query?.Where(x => !string.IsNullOrEmpty(x)).Cast<string>() ?? [];

    public static IEnumerable<string> OmitNullOrEmptyOrWhiteSpace(this IEnumerable<string?> query) => query?.Where(x => !string.IsNullOrWhiteSpace(x)).Cast<string>() ?? [];

    //public static bool IntersectEquals<T>(this IReadOnlyCollection<T> source, IReadOnlyCollection<T> other) => source.Count == other.Count && source.Intersect(other).Count() == other.Count;

    //public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
    //{
    //    TSource[]? bucket = null;

    //    int bucketIndex = 0;

    //    foreach (TSource? item in source)
    //    {
    //        bucket ??= new TSource[size];

    //        bucket[bucketIndex++] = item;

    //        if (bucketIndex != size)
    //            continue;

    //        yield return bucket;

    //        bucket = null;
    //        bucketIndex = 0;
    //    }

    //    if (bucket != null && bucketIndex > 0)
    //        yield return bucket.Take(bucketIndex);
    //}

    //public static bool IntersectEquals<T>(this IReadOnlyCollection<T> source, IReadOnlyCollection<T> other, IEqualityComparer<T> comparer) => source.Count == other.Count && source.Intersect(other, comparer).Count() == other.Count;

    //public static IEnumerable<T> Reverse<T>(this IEnumerable<T> source, bool reverse) => reverse ? source.Reverse() : source;

    //public static IEnumerable<T> SortList<T, TKey>(this IEnumerable<T> input, Func<T, TKey> idProvider, TKey[] ids) where T : class => ids.Select(id => input.FirstOrDefault(x => Equals(idProvider(x), id))).NotNull();

    //public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> input) => input.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key);

    //public static int IndexOf<T>(this IEnumerable<T> input, Func<T, bool> predicate)
    //{
    //    int i = 0;

    //    foreach (T? item in input)
    //    {
    //        if (predicate(item))
    //            return i;

    //        i++;
    //    }

    //    return -1;
    //}

    //public static IEnumerable<TResult> Duplicates<TResult, T>(this IEnumerable<T> input, Func<T, TResult> selector) => input.GroupBy(selector).Where(x => x.Count() > 1).Select(x => x.Key);


    //public static IEnumerable<T> Roll<T>(this T[] target, int maxCount, params T[] source)
    //{
    //    ArgumentNullException.ThrowIfNull(target);
    //    ArgumentNullException.ThrowIfNull(source);

    //    target.AddRange(source);

    //    return target.TakeWhile((_, i) => i < maxCount);
    //}

    public static ICollection<T> AddRange<T>(this ICollection<T> target, params T[] source)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (source is not null)
        {
            foreach (T value in source)
            {
                if (value is not null)
                    target.Add(value);
            }
        }

        return target;
    }

    public static ICollection<T> AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
    {
        foreach (T value in source)
        {
            target.Add(value);
        }
        return target;
    }

    //public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
    //{
    //    Random? random = new();

    //    return enumerable.OrderBy(x => random.Next());
    //}

    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? source) => source ?? [];
    public static T[] OrEmpty<T>(this T[]? source) => source ?? [];

    //public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source) where T : class => source.Where(x => x != null)!;

    //public static IEnumerable<TOut> NotNull<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, TOut?> selector) where TOut : class => source.Select(selector).Where(x => x != null)!;

    //public static TOut[] ProjectAsyncTo<TOut, TIn>(this IEnumerable<TIn> input, Converter<TIn, Task<TOut>> converter) => Task.WhenAll(input.OrEmpty().Select(x => converter(x))).Result;
    //public static TOut[] ProjectAsyncTo<TOut, TIn>(this IEnumerable<TIn> input, Converter<TIn, ValueTask<TOut>> converter) => Task.WhenAll(input.OrEmpty().Select(x => converter(x).AsTask())).Result;


    public static TOut[] ProjectTo<TOut, TIn>(this IEnumerable<TIn> input, Converter<TIn, TOut> converter) => input.OrEmpty().Select(x => converter(x)).ToArray();

    //public static TOut[] ProjectManyTo<TOut, TIn>(this IEnumerable<TIn[]> input, Converter<TIn, TOut> converter) => input.OrEmpty().SelectMany(x => x.Select(z => converter(z))).ToArray();

    public static TIn[] Strip<TIn>(this IEnumerable<TIn> input, params TIn[] other) => input.OrEmpty().Except(other.OrEmpty()).ToArray();
    public static TIn[] Strip<TIn>(this IEnumerable<TIn> input, IEnumerable<TIn> other) => input.Strip(other.OrEmpty());

    //public static TIn[] Merge<TIn>(this IEnumerable<TIn> input, IEnumerable<TIn> other) => input.OrEmpty().Union(other.OrEmpty()).ToArray();
}
