
using MediatR;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Records a church member's decision about publication
/// of their birthday or wedding anniversary celebration.
/// </summary>
/// <param name="Token">
/// The secure invitation token received by the member.
/// </param>
/// <param name="Approved">
/// True when the member consents to publication;
/// false when the member declines.
/// </param>
public sealed record RespondToCelebrationInvitationCommand(
    string Token,
    bool Approved)
    : IRequest<bool>;
