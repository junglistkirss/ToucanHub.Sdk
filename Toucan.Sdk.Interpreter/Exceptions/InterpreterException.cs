namespace Toucan.Sdk.Interpreter.Exceptions;

public sealed class InterpreterException : Exception
{
    public InterpreterException()
    {
    }

    public InterpreterException(string? message) : base(message)
    {
    }

    public InterpreterException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
