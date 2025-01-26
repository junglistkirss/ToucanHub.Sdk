namespace Toucan.Sdk.Store.Services;

public interface IStoreMigrator : IDisposable
{
    Task Migrate();
}
