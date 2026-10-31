
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;

namespace RccgHopeHouse.Application.Features.Celebrations.Queries;

/// <summary>
/// Retrieves the public details of a celebration invitation
/// using the secure token supplied to the member.
/// </summary>
/// <param name="Token">
/// The original invitation token received through the secure link.
/// </param>
public sealed record GetCelebrationInvitationByTokenQuery(
    string Token)
    : IRequest<CelebrationInvitationDetailsDto?>;


