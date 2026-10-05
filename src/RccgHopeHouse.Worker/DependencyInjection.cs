using Microsoft.Extensions.DependencyInjection;
using RccgHopeHouse.Worker.Services;

namespace RccgHopeHouse.Worker;

/// <summary>
/// Provides dependency injection registration for background worker services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the application's background worker services.
    /// </summary>
    /// <param name="services">
    /// The service collection used by the application host.
    /// </param>
    /// <returns>
    /// The same service collection so additional registrations can be chained.
    /// </returns>
    public static IServiceCollection AddWorker(
        this IServiceCollection services)
    {
        services.AddHostedService<YouTubeEventSyncService>();

        return services;
    }
}