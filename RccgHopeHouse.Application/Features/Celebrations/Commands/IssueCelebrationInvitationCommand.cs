
using MediatR;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Requests that a secure celebration invitation be sent
/// to the church member associated with a celebration.
/// </summary>
/// <param name="CelebrationId">
/// Identifier of the celebration.
/// </param>
public sealed record IssueCelebrationInvitationCommand(
    Guid CelebrationId) : IRequest<bool>;
