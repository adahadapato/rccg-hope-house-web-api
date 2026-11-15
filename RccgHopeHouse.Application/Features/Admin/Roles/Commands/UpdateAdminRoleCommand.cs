using MediatR;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Commands;

/// <summary>
/// Updates an existing application's role name and description.
/// </summary>
public sealed record UpdateAdminRoleCommand(
    string RoleId,
    string Name,
    string? Description)
    : IRequest;