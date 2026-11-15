using MediatR;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Commands;

/// <summary>
/// Deletes an application role that is not protected
/// and is not currently assigned to any users.
/// </summary>
public sealed record DeleteAdminRoleCommand(
    string RoleId)
    : IRequest;
