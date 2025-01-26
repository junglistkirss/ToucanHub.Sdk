namespace Toucan.Sdk.Core.Handlers.Exceptions;

public sealed class HandleException : Exception
{
    public IMessage SourceMessage { get; internal set; } = default!;

    public HandleException()
    {
    }

    public HandleException(string? message) : base(message)
    {
    }

    public HandleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
