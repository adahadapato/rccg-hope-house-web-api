using MediatR;

using RccgHopeHouse.Application.Features.Admin.Users.Dtos;

namespace RccgHopeHouse.Application.Features.Admin.Users.Queries;

/// <summary>
/// Gets all application users for the administration area.
/// </summary>
public sealed record GetAdminUsersQuery
    : IRequest<IReadOnlyList<AdminUserDto>>;