namespace Toucan.Sdk.EventSourcing;

public interface IStorageTransaction : IDisposable, IAsyncDisposable
{
    void Commit();
    void Rollback();
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
}
