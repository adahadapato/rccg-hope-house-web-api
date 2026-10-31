
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Represents a member's decision to decline
/// publication of their celebration.
/// </summary>
/// <param name="CelebrationId">
/// The unique identifier of the celebration.
/// </param>
/// <remarks>
/// Declining publication means that the member does not
/// consent to having their celebration published.
///
/// The celebration remains in the database for
/// administrative record-keeping but must not
/// appear in public celebration galleries.
///
/// The caller must be authorised to respond
/// on behalf of the associated member.
/// </remarks>
public sealed record DisapproveCelebrationCommand(
    Guid CelebrationId
) : IRequest<CelebrationDto>;
