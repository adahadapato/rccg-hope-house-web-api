using MediatR;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Represents a request for prayer request workflow statistics
/// used by the administration dashboard.
/// </summary>
public record GetPrayerRequestStatsQuery
    : IRequest<PrayerRequestStatsDto>;

/// <summary>
/// Represents prayer request counts grouped by pastoral
/// workflow status.
/// </summary>
/// <param name="PendingCount">
/// Number of prayer requests currently awaiting attention.
/// </param>
/// <param name="InProgressCount">
/// Number of prayer requests currently being attended to.
/// </param>
/// <param name="ResolvedCount">
/// Number of prayer requests that have been resolved.
/// </param>
/// <param name="TotalCount">
/// Total number of prayer requests.
/// </param>
public record PrayerRequestStatsDto(
    int PendingCount,
    int InProgressCount,
    int ResolvedCount,
    int TotalCount);