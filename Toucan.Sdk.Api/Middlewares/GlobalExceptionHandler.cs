using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Toucan.Sdk.Api.Extensions;

namespace Toucan.Sdk.Api.Middlewares;

internal class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogCritical(exception, "Exception occurred: {Message}", exception.Message);

        await ApiResultsHelper
            .InternalErrorMessage("Fallback: An error occurred")
            .ExecuteAsync(httpContext);

        return true;
    }
}