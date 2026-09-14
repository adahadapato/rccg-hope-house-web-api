using MediatR;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Query to retrieve paginated prayer requests for admin dashboard.
/// Supports filtering by status and chronological sorting.
/// </summary>
public record GetPrayerRequestsQuery(
    PrayerRequestStatus? Status = null,
    int Skip = 0,
    int Take = 20) : IRequest<IReadOnlyList<PrayerRequestDto>>;