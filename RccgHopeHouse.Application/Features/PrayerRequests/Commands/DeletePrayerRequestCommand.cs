using MediatR;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

/// <summary>
/// Represents an administrator's request to permanently delete
/// a prayer request.
/// </summary>
/// <param name="Id">
/// The unique identifier of the prayer request to delete.
/// </param>
/// <remarks>
/// This operation is intended for authorised administrative use.
/// </remarks>
public record DeletePrayerRequestCommand(
    Guid Id) : IRequest<Unit>;