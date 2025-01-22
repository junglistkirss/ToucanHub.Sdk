using System.Text.Json.Serialization;

namespace Toucan.Sdk.Contracts.Query.Filters;

public interface IFilterNode<T> 
{
   public T? Filter { get;  }

}

