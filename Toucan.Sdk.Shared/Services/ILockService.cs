using Toucan.Sdk.Contracts.Wrapper;
using Toucan.Sdk.Shared.Models;

namespace Toucan.Sdk.Shared.Services;

public interface ILockService
{
    ValueTask<Result<LockToken?>> TryReserve<T>(RefToken owner, T value, CancellationToken cancellationToken) where T : ITypedLock;
    ValueTask<Result> TryRelease(RefToken owner, LockToken token, CancellationToken cancellationToken);
    ValueTask<Result> TryComplete(LockToken token, CancellationToken cancellationToken, DomainId? domainId = null);

}
