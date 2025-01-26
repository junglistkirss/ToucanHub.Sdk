using Toucan.Sdk.Shared.Models;

namespace Toucan.Sdk.Core.Aggregates.Abstractions;

public delegate void Mutator(ISignal Signal, EventMessage Message);
public delegate void Mutator<T>(ISignal Signal, T Message) where T : EventMessage;

public abstract class EventAggregator : IEventSourced<Commit>
{
    private record class EventMutation(Predicate<Type> MessageTypePredicate, Mutator Action);

    private readonly HashSet<EventMutation> mutations = [];

    void IEventSourced<Commit>.ApplyMutation(Commit commit)
    {
        try
        {
            foreach (EventMessage message in commit.Messages)
                foreach (EventMutation evolve in mutations.Where(x => x.MessageTypePredicate(message.GetType())))
                    evolve?.Action(commit.Headers, message);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Incompatible/Unknown event", ex);
        }
    }

    protected void OnAny(Mutator mutate)
       => mutations.Add(new EventMutation(_ => true, mutate));

    protected void On<T>(Mutator<T> mutate) where T : EventMessage
        => mutations.Add(new EventMutation(x => x == typeof(T), (s, m) => mutate(s, (T)m)));

    protected void OnAssignableTo<T>(Mutator<T> mutate) where T : EventMessage
        => mutations.Add(new EventMutation(x => x.IsAssignableTo(typeof(T)), (s, m) => mutate(s, (T)m)));

    protected void OnSomeAssignableTo<T>(Mutator<T> mutate) where T : EventMessage
        => mutations.Add(new EventMutation(x => x.IsAssignableTo(typeof(T)), (s, m) => mutate(s, (T)m)));

}
