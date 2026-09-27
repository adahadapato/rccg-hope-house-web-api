using MediatR;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Activates or deactivates an application user.
/// </summary>
public sealed record SetAdminUserStatusCommand(
    string UserId,
    string CurrentUserId,
    bool IsActive)
    : IRequest;
