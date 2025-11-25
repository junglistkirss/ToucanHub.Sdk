using System.Runtime.Serialization;

namespace ToucanHub.Sdk.Contracts.Messages;

[DataContract, Serializable]
public sealed record class EmptyMessage : IMessage
{
    public static readonly EmptyMessage Instance = new();
}
