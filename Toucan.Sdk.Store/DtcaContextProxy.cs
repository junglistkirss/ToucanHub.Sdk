namespace Toucan.Sdk.Store;

public abstract class WriteContextProxy<TContext> : ReadContextProxy<TContext>, IWriteContextProxy
    where TContext : IWriteContextProxy
{
    protected WriteContextProxy(TContext context) : base(context)
    {
    }

    public abstract void EnsureAdd<T>(T entity) where T : class;
    public abstract void EnsureDelete<T>(T entity) where T : class;
    public abstract void EnsureUpdate<T>(T entity) where T : class;
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => Context.SaveChangesAsync(cancellationToken);

}
public abstract class ReadContextProxy<TContext> : IReadContextProxy
    where TContext : IReadContextProxy
{
    private bool disposedValue;

    protected TContext Context { get; }

    protected ReadContextProxy(TContext context)
    {
        Context = context;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                Context.Dispose();

            // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
            // TODO: affecter aux grands champs une valeur null
            disposedValue = true;
        }
    }
    public void Dispose()
    {
        // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
