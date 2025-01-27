namespace Toucan.Sdk.Contracts.Query.Filters;

public interface IFilterNode<T>
{
    public T? Filter { get; }

}

