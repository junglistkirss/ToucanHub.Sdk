using System.Diagnostics.CodeAnalysis;

namespace ToucanHub.Sdk.Pipeline.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public sealed class FlowException : Exception
{
    public FlowException()
    {
    }

    public FlowException(string? message) : base(message)
    {
    }

    public FlowException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}