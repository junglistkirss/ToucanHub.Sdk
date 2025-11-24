namespace ToucanHub.Sdk.Contracts.Query;

public interface IFiltered<TFilter>
{
    public TFilter Filter { get; }
}
