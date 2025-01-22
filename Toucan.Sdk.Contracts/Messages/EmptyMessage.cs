using System.Runtime.Serialization;

namespace Toucan.Sdk.Contracts.Messages;

[DataContract, Serializable]
public sealed record class EmptyMessage : IMessage
{
    public static readonly EmptyMessage Instance = new();
}
