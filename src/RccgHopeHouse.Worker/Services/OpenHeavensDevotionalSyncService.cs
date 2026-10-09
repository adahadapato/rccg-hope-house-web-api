using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Application.Features.Devotionals.Commands;

namespace RccgHopeHouse.Worker.Services;

/// <summary>
/// Schedules automatic Open Heavens devotional synchronisation
/// according to United Kingdom local time.
/// </summary>
public sealed class OpenHeavensDevotionalSyncService : BackgroundService
{
    private readonly ILogger<OpenHeavensDevotionalSyncService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private static readonly TimeZoneInfo UkTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/London");

    private static readonly TimeOnly[] ScheduledTimes =
    [
        new(0, 5),
        new(1, 0),
        new(3, 0),
        new(6, 0)
    ];

    /// <summary>
    /// Initializes the Open Heavens devotional scheduler.
    /// </summary>
    public OpenHeavensDevotionalSyncService(
        IServiceScopeFactory scopeFactory,
        ILogger<OpenHeavensDevotionalSyncService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Executes the scheduled synchronisation loop.
    /// </summary>
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Open Heavens devotional scheduler started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = TimeZoneInfo.ConvertTime(
                    DateTimeOffset.UtcNow,
                    UkTimeZone);

                var nextRun = GetNextRun(now);

                var delay = nextRun - DateTimeOffset.UtcNow;

                if (delay > TimeSpan.Zero)
                {
                    _logger.LogInformation(
                        "Next Open Heavens synchronisation: {NextRun}",
                        nextRun);

                    await Task.Delay(delay, stoppingToken);
                }

                try
                {
                    await SynchronizeAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Open Heavens scheduled synchronisation failed.");
                }
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Expected application shutdown.
        }

        _logger.LogInformation(
            "Open Heavens devotional scheduler stopped.");
    }

    /// <summary>
    /// Sends the synchronisation command through MediatR
    /// using a scoped service provider.
    /// </summary>
    private async Task SynchronizeAsync(
        CancellationToken cancellationToken)
    {
        var ukNow = TimeZoneInfo.ConvertTime(
            DateTimeOffset.UtcNow,
            UkTimeZone);

        var devotionalDate = DateOnly.FromDateTime(
            ukNow.DateTime);

        _logger.LogInformation(
            "Starting Open Heavens synchronisation for {Date}.",
            devotionalDate);

        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider
            .GetRequiredService<IMediator>();

        var result = await mediator.Send(
            new SynchronizeOpenHeavensDevotionalsCommand(
                devotionalDate),
            cancellationToken);

        _logger.LogInformation(
            "Open Heavens synchronisation completed. " +
            "Date: {Date}; Status: {Status}; Message: {Message}",
            result.DevotionalDate,
            result.Status,
            result.Message);
    }

    /// <summary>
    /// Calculates the next scheduled execution time,
    /// accounting for UK daylight saving transitions.
    /// </summary>
    private static DateTimeOffset GetNextRun(
        DateTimeOffset now)
    {
        for (var dayOffset = 0; dayOffset <= 2; dayOffset++)
        {
            var date = DateOnly.FromDateTime(now.DateTime)
                .AddDays(dayOffset);

            foreach (var time in ScheduledTimes)
            {
                var localDateTime = date.ToDateTime(
                    time,
                    DateTimeKind.Unspecified);

                if (UkTimeZone.IsInvalidTime(localDateTime))
                {
                    continue;
                }

                var offset = UkTimeZone.GetUtcOffset(
                    localDateTime);

                var candidate = new DateTimeOffset(
                    localDateTime,
                    offset);

                if (candidate > now)
                {
                    return candidate;
                }
            }
        }

        throw new InvalidOperationException(
            "Unable to calculate the next Open Heavens execution time.");
    }
}