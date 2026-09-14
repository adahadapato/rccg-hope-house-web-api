using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

/// <summary>
/// Command to update the pastoral status of a prayer request.
/// Admin-only operation for tracking prayer team workflow.
/// </summary>
public record UpdatePrayerRequestStatusCommand(
    Guid Id,
    PrayerRequestStatus NewStatus,
    string? PastoralNote) : IRequest<Unit>;