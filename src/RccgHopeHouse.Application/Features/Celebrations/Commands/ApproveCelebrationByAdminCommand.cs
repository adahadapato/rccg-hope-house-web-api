
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Represents an administrator's request to approve a member's celebration for publication.
/// </summary>
/// <param name="CelebrationId">The unique identifier of the celebration to approve.</param>
/// <remarks>
/// The member must have approved publication before administrator approval is permitted.
/// Successful approval changes the celebration status from ApprovedByMember to ReadyToPublish.
/// This command must only be accessible to authorised administrators.
/// </remarks>
public sealed record ApproveCelebrationByAdminCommand(Guid CelebrationId) : IRequest<CelebrationDto>;
