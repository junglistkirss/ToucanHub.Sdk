namespace Toucan.Sdk.Utils;

/// <summary>
/// Typed Activator Factory
/// </summary>
/// <typeparam name="T"></typeparam>
public delegate T Factory<T>(IServiceProvider provider);
public delegate T KeyFactory<T, TKey>(IServiceProvider provider, TKey key);

public delegate T Provide<T>();
public delegate T KeyProvide<T, TKey>(TKey key);

