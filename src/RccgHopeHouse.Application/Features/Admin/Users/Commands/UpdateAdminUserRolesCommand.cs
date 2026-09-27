using MediatR;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Updates the roles assigned to an application user.
/// </summary>
public sealed record UpdateAdminUserRolesCommand(
    string UserId,
    string CurrentUserId,
    IReadOnlyCollection<string> Roles)
    : IRequest;
