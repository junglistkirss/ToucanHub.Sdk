namespace Toucan.Sdk.Interpreter.Exceptions;

public sealed class InterpreterInteropException : Exception
{
    public InterpreterInteropException()
    {
    }

    public InterpreterInteropException(string? message) : base(message)
    {
    }

    public InterpreterInteropException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
