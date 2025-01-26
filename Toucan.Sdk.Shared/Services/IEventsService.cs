using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Query;
using Toucan.Sdk.Contracts.Query.Page;
using Toucan.Sdk.Contracts.Wrapper;
using Toucan.Sdk.Shared.Messages;
using Toucan.Sdk.Shared.Models;

namespace Toucan.Sdk.Shared.Services;

public interface IEventsService<TStreamKey, TEvent>
    where TStreamKey : notnull
    where TEvent : notnull
{
    Task<Results<EventInfo>> GetEvents(TStreamKey key, ICollectionQuery<BaseEventsFilter, SearchEvents, Slug> query, CancellationToken cancellationToken = default);
    Task<Results<DomainId>> GetStreamKeys(Pagination page, CancellationToken cancellationToken = default);
}


