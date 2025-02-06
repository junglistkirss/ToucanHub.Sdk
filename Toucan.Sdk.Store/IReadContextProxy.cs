namespace Toucan.Sdk.Store;

public interface IContextTransaction : IDisposable, IAsyncDisposable
{
    void Commit();
    void Rollback();
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
}


public interface IReadContextProxy : IDisposable {
    Task<IContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

public interface IWriteContextProxy : IReadContextProxy
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}