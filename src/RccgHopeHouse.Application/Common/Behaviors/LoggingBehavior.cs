using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace RccgHopeHouse.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs request execution time and outcome.
/// Useful for monitoring, debugging, and performance analysis.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the logging behavior.
    /// </summary>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Logs request start, duration, and outcome.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogDebug("Starting request {RequestName} {@Request}", requestName, request);

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
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Failed request {RequestName} after {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            throw; // Re-throw to be caught by global exception handler
        }
    }
}