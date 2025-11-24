namespace ToucanHub.Sdk.Utils;

public static class Factory
{
    public static Factory<T> Createrovider<T>(Provider<T> provide) => (_) => provide();
    public static Factory<T> CreateResolver<T>(Resolver<T> resolve) => (s) => resolve(s);

    public delegate T Resolver<T>(IServiceProvider serviceProvider);
    public delegate T Provider<T>();

    public static KeyFactory<T, TKey> CreateKeyProvider<T, TKey>(KeyProvider<T, TKey> provide) => (_, key) => provide(key);
    public static KeyFactory<T, TKey> CreateKeyResolver<T, TKey>(KeyResolver<T, TKey> resolve) => (s, k) => resolve(s, k);

    public delegate T KeyResolver<T, TKey>(IServiceProvider serviceProvider, TKey key);
    public delegate T KeyProvider<T, TKey>(TKey key);
}

