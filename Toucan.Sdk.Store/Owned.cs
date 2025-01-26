namespace Toucan.Sdk.Store;

public abstract record class Owned<TOwner>
    where TOwner : class
{ }