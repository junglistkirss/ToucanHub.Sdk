namespace Toucan.Sdk.Pipeline.Exceptions;

[Serializable]
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