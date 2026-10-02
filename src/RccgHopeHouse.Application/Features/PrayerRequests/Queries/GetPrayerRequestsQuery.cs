using MediatR;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Represents a request to retrieve a paginated collection
/// of prayer requests for administrative management.
/// </summary>
/// <param name="Status">
/// Optional pastoral workflow status used to filter the results.
/// When <c>null</c>, prayer requests of all statuses are returned.
/// </param>
/// <param name="Skip">
/// The number of prayer requests to skip.
/// </param>
/// <param name="Take">
/// The maximum number of prayer requests to return.
/// </param>
public record GetPrayerRequestsQuery(
    PrayerRequestStatus? Status = null,
    int Skip = 0,
    int Take = 20)
    : IRequest<IReadOnlyList<PrayerRequestDto>>;