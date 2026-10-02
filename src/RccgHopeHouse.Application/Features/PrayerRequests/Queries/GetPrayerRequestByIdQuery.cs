using MediatR;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Represents a request to retrieve a single prayer request
/// for administrative viewing.
/// </summary>
/// <param name="Id">
/// The unique identifier of the prayer request.
/// </param>
/// <remarks>
/// Returns the complete prayer request information required by
/// the administration interface, including pastoral workflow
/// information and notes.
/// </remarks>
public record GetPrayerRequestByIdQuery(
    Guid Id) : IRequest<PrayerRequestDto>;