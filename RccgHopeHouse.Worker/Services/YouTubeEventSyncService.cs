using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

namespace RccgHopeHouse.Worker.Services;

/// <summary>
/// Synchronises configured YouTube channels once daily at midnight UK time.
/// Does not run immediately when the host starts.
/// </summary>
public sealed class YouTubeEventSyncService : BackgroundService
{
    private static readonly TimeZoneInfo UkTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/London");

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<YouTubeEventSyncService> _logger;

    /// <summary>Initialises the scheduled YouTube synchronization worker.</summary>
    public YouTubeEventSyncService(
        IServiceScopeFactory scopeFactory,
        ILogger<YouTubeEventSyncService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Waits until the next UK midnight, then synchronises once.
    /// Recalculates each day to account for GMT/BST transitions.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("YouTube synchronization scheduled daily at 00:00 UK time.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var nowUtc = DateTimeOffset.UtcNow;
                var ukNow = TimeZoneInfo.ConvertTime(nowUtc, UkTimeZone);
                var nextDate = DateOnly.FromDateTime(ukNow.DateTime).AddDays(1);
                var nextLocalMidnight = nextDate.ToDateTime(
                    TimeOnly.MinValue,
                    DateTimeKind.Unspecified);
                var nextUtc = TimeZoneInfo.ConvertTimeToUtc(
                    nextLocalMidnight,
                    UkTimeZone);

                _logger.LogInformation(
                    "Next YouTube synchronization scheduled for {NextRunUtc} UTC.",
                    nextUtc);

                await Task.Delay(nextUtc - nowUtc.UtcDateTime, stoppingToken);
                await SynchronizeAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Expected during host shutdown.
        }

        _logger.LogInformation("YouTube synchronization service stopped.");
    }

    /// <summary>
    /// Runs the existing broadcast synchronisation command within a DI scope.
    /// Failures are logged without stopping the background service.
    /// </summary>
    private async Task SynchronizeAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting scheduled YouTube broadcast synchronization.");
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(
                new SynchronizeServiceBroadcastsCommand(),
                cancellationToken);

            _logger.LogInformation(
                "YouTube synchronization completed. Videos examined: {VideosExamined}. " +
                "Created: {BroadcastsCreated}. Updated: {BroadcastsUpdated}. " +
                "Skipped: {BroadcastsSkipped}.",
                result.VideosExamined,
                result.BroadcastsCreated,
                result.BroadcastsUpdated,
                result.BroadcastsSkipped);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Expected during host shutdown.
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Scheduled YouTube synchronization failed.");
        }
    }
}
