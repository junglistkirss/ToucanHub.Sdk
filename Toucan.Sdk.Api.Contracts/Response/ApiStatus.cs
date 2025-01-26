namespace Toucan.Sdk.Api.Contracts.Response;

public enum ApiStatus
{
    NonCanonical,
    //Canonical,
    Success,
    Failure,
    //Pending,
    Timeout,
    Unauthorized,
    Forbidden,
    NotFound,
    InternalError,
}
