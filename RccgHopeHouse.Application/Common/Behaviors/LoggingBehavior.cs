using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace RccgHopeHouse.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs request execution time and outcome.
/// Useful for monitoring, debugging, and performance analysis.
/// </summary>
public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the logging behavior.
    /// </summary>
    /// <param name="logger">
    /// Logger used to record request execution information.
    /// </param>
    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes the request pipeline and logs its duration and outcome.
    /// </summary>
    /// <param name="request">
    /// The MediatR request being executed.
    /// </param>
    /// <param name="next">
    /// Delegate used to invoke the next pipeline behavior or request handler.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the request.
    /// </param>
    /// <returns>
    /// The response returned by the request handler.
    /// </returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogDebug(
            "Starting request {RequestName} {@Request}",
            requestName,
            request);

        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogDebug(
                "Completed request {RequestName} in {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();

            _logger.LogDebug(
                "Request {RequestName} was cancelled after {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "Failed request {RequestName} after {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            // Re-throw so the application's central exception handling
            // pipeline can process genuine failures.
            throw;
        }
    }
}