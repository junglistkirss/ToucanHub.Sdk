namespace Toucan.Sdk.Shared.Models;

public interface ITypedLock
{
    TimeSpan GetLockLifetime();
    string GetLockType();
    string GetHash();
}
