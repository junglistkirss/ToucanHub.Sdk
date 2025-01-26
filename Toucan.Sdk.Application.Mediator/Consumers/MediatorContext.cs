namespace Toucan.Sdk.Application.Mediator.Consumers;

public interface IMediatorConsumer<T>
    where T : class
{
    ValueTask Consume(MediatorContext<T> context);

}

public interface IMediatorConsumer<T, TReposne>
    where T : class
    where TReposne : class
{
    ValueTask<TReposne> Consume(MediatorContext<T> context);

}

public record class MediatorContext<T> where T : class
{
    public required T Message { get; init; }
    public required CancellationToken CancellationToken { get; init; }

}
