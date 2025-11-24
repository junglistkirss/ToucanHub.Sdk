namespace Toucan.Sdk.Contracts.Wrapper;

public sealed class NoValue
{
    private NoValue() { }
    public static readonly NoValue Instance = new();
    public override bool Equals(object? _) => ReferenceEquals(this, Instance);
    public override int GetHashCode() => 16777619; // FNV prime
}
