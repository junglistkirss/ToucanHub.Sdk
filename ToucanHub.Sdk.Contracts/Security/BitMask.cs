namespace ToucanHub.Sdk.Contracts.Security;

public readonly struct BitMask(int mask)
{
    private readonly int _mask = mask;
    public bool Intersects(BitMask other) => (_mask & other._mask) != 0;
}
