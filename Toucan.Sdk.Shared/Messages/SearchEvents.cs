using System.Text.Json.Serialization;
using Toucan.Sdk.Contracts.Query;
using Toucan.Sdk.Contracts.Query.Filters;

namespace Toucan.Sdk.Shared.Messages;

public readonly record struct SearchEvents : IMessage
{
    public StringFilter? Origin { get; init; }
    public StringFilter? Sender { get; init; }
    public StringFilter? EventType { get; init; }
    public DateFilter? EventDateAfter { get; init; }
    public DateFilter? EventDateBefore { get; init; }
}

public readonly record struct SearchStreams : IMessage
{
    public StringFilter? Name { get; init; }
    public DateFilter? StreamDateAfter { get; init; }
    public DateFilter? StreamDateBefore { get; init; }
}




[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(EventsFilterNode), typeDiscriminator: "node")]
[JsonDerivedType(typeof(EventsFilterGroup), typeDiscriminator: "group")]

public abstract record class BaseEventsFilter : BaseFilterNode<SearchEvents>
{
}

public sealed record class EventsFilterNode : BaseEventsFilter, IFilterNode<SearchEvents>
{
    public SearchEvents Filter { get; init; } = default!;
}
public sealed record class EventsFilterGroup : BaseEventsFilter, IFilterGroup<SearchEvents, BaseEventsFilter>
{
    public FilterAggregator Aggregator { get; init; } = FilterAggregator.AND;

    public BaseEventsFilter[] Nodes { get; init; } = [];
}


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(StreamsFilterNode), typeDiscriminator: "node")]
[JsonDerivedType(typeof(StreamsFilterGroup), typeDiscriminator: "group")]

public abstract record class BaseStreamsFilter : BaseFilterNode<SearchStreams>
{
}

public sealed record class StreamsFilterNode : BaseStreamsFilter, IFilterNode<SearchStreams>
{
    public SearchStreams Filter { get; init; } = default!;
}
public sealed record class StreamsFilterGroup : BaseStreamsFilter, IFilterGroup<SearchStreams, BaseStreamsFilter>
{
    public FilterAggregator Aggregator { get; init; } = FilterAggregator.AND;

    public BaseStreamsFilter[] Nodes { get; init; } = [];
}