namespace Toucan.Sdk.Store;

public interface IReadContextProxy : IDisposable { }

public interface IWriteContextProxy : IReadContextProxy
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}