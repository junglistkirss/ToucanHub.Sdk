namespace Toucan.Sdk.Shared.Models;

public readonly record struct LockToken
{
    public string HashKey { get; }

    public LockToken(string hashKey)
    {
        if (string.IsNullOrEmpty(hashKey))
            throw new ArgumentNullException(nameof(hashKey));

        HashKey = hashKey;
    }
}
