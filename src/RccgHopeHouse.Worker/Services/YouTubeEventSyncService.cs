using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

namespace RccgHopeHouse.Worker.Services;

/// <summary>
/// Periodically triggers synchronization of configured YouTube channels
/// with church service broadcasts.
/// </summary>
public sealed class YouTubeEventSyncService : BackgroundService
{
    private static readonly TimeSpan SynchronizationInterval =
        TimeSpan.FromHours(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<YouTubeEventSyncService> _logger;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="YouTubeEventSyncService"/> class.
    /// </summary>
    /// <param name="scopeFactory">
    /// Factory used to create dependency injection scopes for each
    /// synchronization operation.
    /// </param>
    /// <param name="logger">
    /// Logger used to record synchronization activity and failures.
    /// </param>
    public YouTubeEventSyncService(
        IServiceScopeFactory scopeFactory,
        ILogger<YouTubeEventSyncService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Executes the YouTube synchronization process for the lifetime
    /// of the host application.
    /// </summary>
    /// <param name="stoppingToken">
    /// Token that is triggered when the host application is stopping.
    /// </param>
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "YouTube event synchronization service started.");

        /*
         * Run once immediately when the application starts.
         *
         * Subsequent synchronization runs occur at the configured
         * interval below.
         */
        await SynchronizeAsync(stoppingToken);

        using var timer =
            new PeriodicTimer(SynchronizationInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await SynchronizeAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Normal application shutdown.
        }

        _logger.LogInformation(
            "YouTube event synchronization service stopped.");
    }

    /// <summary>
    /// Creates a dependency injection scope and sends the
    /// service broadcast synchronization command through MediatR.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token used to cancel the synchronization operation.
    /// </param>
    private async Task SynchronizeAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Starting YouTube service broadcast synchronization.");

            using var scope =
                _scopeFactory.CreateScope();

            var mediator =
                scope.ServiceProvider
                    .GetRequiredService<IMediator>();

            var result =
                await mediator.Send(
                    new SynchronizeServiceBroadcastsCommand(),
                    cancellationToken);

            _logger.LogInformation(
                "YouTube service broadcast synchronization completed. " +
                "Videos examined: {VideosExamined}. " +
                "Broadcasts created: {BroadcastsCreated}. " +
                "Broadcasts updated: {BroadcastsUpdated}. " +
                "Broadcasts skipped: {BroadcastsSkipped}.",
                result.VideosExamined,
                result.BroadcastsCreated,
                result.BroadcastsUpdated,
                result.BroadcastsSkipped);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            // Application shutdown. Allow the host to stop normally.
        }
        catch (Exception exception)
        {
            /*
             * A failed YouTube synchronization must not terminate
             * the API process. The next scheduled run can try again.
             */
            _logger.LogError(
                exception,
                "YouTube service broadcast synchronization failed.");
        }
    }
}