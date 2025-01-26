namespace Toucan.Sdk.Application.Exceptions;

public sealed class DispatchException : Exception
{
    public object? ObjectSource { get; set; } = null;

    public DispatchException()
    {
    }

    public DispatchException(string? message) : base(message)
    {
    }

    public DispatchException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
