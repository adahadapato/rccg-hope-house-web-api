using MediatR;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Query to get dashboard statistics for the prayer team.
/// Returns counts by status for quick workflow overview.
/// </summary>
public record GetPrayerRequestStatsQuery : IRequest<PrayerRequestStatsDto>;

public record PrayerRequestStatsDto(
    int PendingCount,
    int InProgressCount,
    int ResolvedCount,
    int TotalCount);