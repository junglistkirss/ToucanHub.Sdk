namespace Toucan.Sdk.Contracts.Wrapper;

public sealed class NoValue
{
    private NoValue() { }
    public static readonly NoValue Value = new();
}
