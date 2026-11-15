
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Represents a member's approval for publication
/// of their birthday or wedding anniversary celebration.
/// </summary>
/// <param name="CelebrationId">
/// The unique identifier of the celebration.
/// </param>
/// <remarks>
/// This command records the member's consent to publication.
///
/// It does not constitute administrator approval and
/// does not publish the celebration automatically.
///
/// The request must be submitted only after the member's
/// identity and authority to respond have been verified.
/// </remarks>
public sealed record ApproveCelebrationPublicationCommand(
    Guid CelebrationId
) : IRequest<CelebrationDto>;
