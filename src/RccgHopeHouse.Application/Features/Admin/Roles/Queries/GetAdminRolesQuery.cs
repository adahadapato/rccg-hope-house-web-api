using MediatR;
using RccgHopeHouse.Application.Features.Admin.Roles.Dtos;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Queries;

/// <summary>
/// Gets all application roles available for assignment
/// through the administration area.
/// </summary>
public sealed record GetAdminRolesQuery
    : IRequest<IReadOnlyList<AdminRoleDto>>;
