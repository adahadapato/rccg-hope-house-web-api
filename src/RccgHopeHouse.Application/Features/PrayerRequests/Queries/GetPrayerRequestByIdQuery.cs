using MediatR;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Query to retrieve a single prayer request for admin detail view.
/// Loads full metadata including pastoral notes.
/// </summary>
public record GetPrayerRequestByIdQuery(Guid Id) : IRequest<PrayerRequestDto>;