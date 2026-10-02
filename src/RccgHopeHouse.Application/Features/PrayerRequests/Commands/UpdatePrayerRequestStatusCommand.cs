using MediatR;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

/// <summary>
/// Represents an administrator's request to update the pastoral
/// workflow status of a prayer request.
/// </summary>
/// <param name="Id">
/// The unique identifier of the prayer request to update.
/// </param>
/// <param name="NewStatus">
/// The new pastoral workflow status.
/// </param>
/// <param name="PastoralNote">
/// An optional internal pastoral note associated with the request.
/// </param>
/// <remarks>
/// The command supports the existing prayer request workflow:
/// Pending, InProgress, and Resolved.
/// </remarks>
public record UpdatePrayerRequestStatusCommand(
    Guid Id,
    PrayerRequestStatus NewStatus,
    string? PastoralNote) : IRequest<Unit>;