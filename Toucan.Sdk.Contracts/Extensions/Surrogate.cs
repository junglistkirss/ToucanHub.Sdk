using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.Extensions;

public static class Surrogate
{
    public static T ToSource<T>(this ITarget<T> o) => o.ToTarget();
    public static IEnumerable<T> ToSources<T>(this IEnumerable<ITarget<T>> o) => o.Select(ToSource);

    public static TSurrogate From<T, TSurrogate>(T input, TSurrogate surrogate)
        where TSurrogate : ISource<T>
    {
        surrogate.FromSource(input);
        return surrogate;
    }
    public static TSurrogate FromActivator<T, TSurrogate>(T input, Func<Type, TSurrogate> activator)
        where TSurrogate : ISource<T>
        => input is not null ? From(input, activator(input.GetType())) : throw new ArgumentNullException(nameof(input));

    public static TSurrogate? FromActivatorOrNull<T, TSurrogate>(T? input, Func<Type, TSurrogate> activator)
        where TSurrogate : ISource<T>
        => input is not null ? FromActivator(input, activator) : default;



    public static TSurrogate? FromOrNull<T, TSurrogate>(T? input)
        where TSurrogate : ISource<T>, new()
        => input is not null ? From<T, TSurrogate>(input, new()) : default;


    public static IEnumerable<TSurrogate> Froms<T, TSurrogate>(IEnumerable<T> input)
       where TSurrogate : ISource<T>, new()
       => input.Select(From<T, TSurrogate>);


    public static TSurrogate From<T, TSurrogate>(T input)
        where TSurrogate : ISource<T>, new()
        => From<T, TSurrogate>(input, new());

    public static TSurrogate FromDefault<T, TSurrogate>()
        where TSurrogate : ISource<T>, new()
        where T : new()
    {
        TSurrogate surrogate = new();
        surrogate.FromSource(new T());
        return surrogate;
    }
}
