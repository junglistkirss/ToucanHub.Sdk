using System.Runtime.Serialization;

namespace Toucan.Sdk.Contracts.Messages;

[DataContract, Serializable]
public readonly record struct Metadata
{
    public static readonly Metadata Empty = new();

    public Metadata() { }

    [DataMember]
    public string Key { get; init; } = string.Empty;

    [DataMember]
    public string? Value { get; init; } = null;

    public static Metadata Create<T>(string key, T? value)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));

        string? output = key.Trim();
        if(string.IsNullOrEmpty(output))
            throw new ArgumentException("Invalid metadata key",nameof(key));

        return Empty with
        {
            Key = output,
            Value = value?.ToString(),
        };
    }
}
