namespace Toucan.Sdk.Async;

public static class CancellationTokenExtensions
{
    public static CancellationToken WithTimeout(this CancellationToken cancellationToken, int timeout_ms)
    {
        CancellationTokenSource ts = new(timeout_ms);
        CancellationTokenSource final = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ts.Token);
        return final.Token;
    }

    public static CancellationToken WithTimeout(this CancellationToken cancellationToken, TimeSpan timeout_ms)
    {
        CancellationTokenSource ts = new(timeout_ms);
        CancellationTokenSource final = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ts.Token);
        return final.Token;
    }

    public static CancellationToken WrapTimeout(this CancellationToken cancellationToken, CancellationToken other)
    {
        CancellationTokenSource final = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, other);
        return final.Token;
    }
}

