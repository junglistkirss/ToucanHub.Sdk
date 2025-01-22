namespace Toucan.Sdk.Contracts.Registry;

[Serializable]
public sealed class TypeNameNotFoundException(string? message = null, Exception? inner = null) : Exception(message, inner)
{
}
