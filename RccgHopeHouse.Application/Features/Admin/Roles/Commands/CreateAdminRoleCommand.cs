using MediatR;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Commands;

/// <summary>
/// Creates a new application role.
/// </summary>
public sealed record CreateAdminRoleCommand(
    string Name,
    string? Description)
    : IRequest<string>;