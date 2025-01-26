namespace Toucan.Sdk.Shared.Models;


public interface IIdentified<TId>
    where TId : struct
{
    TId Identity { get; }
}


//public interface ISubAggregateIdentified<TId> : IIdentified<TId>
//    where TId : struct
//{
//    Task Init(TId id);
//}



//public interface IEventSourced<TId> : IIdentified<TId>
//    where TId : struct
//{

//}