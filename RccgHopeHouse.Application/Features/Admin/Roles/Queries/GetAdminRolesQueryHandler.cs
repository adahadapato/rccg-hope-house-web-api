using MediatR;
using RccgHopeHouse.Application.Features.Admin.Roles.Dtos;
using RccgHopeHouse.Core.Interfaces.Admin;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Queries;

/// <summary>
/// Handles <see cref="GetAdminRolesQuery"/>.
/// </summary>
public sealed class GetAdminRolesQueryHandler
    : IRequestHandler<GetAdminRolesQuery, IReadOnlyList<AdminRoleDto>>
{
    private readonly IAdminRepository _adminRepository;

    public GetAdminRolesQueryHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<IReadOnlyList<AdminRoleDto>> Handle(
        GetAdminRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles =
            await _adminRepository.GetRolesAsync(
                cancellationToken);

        return roles
            .Select(role => new AdminRoleDto(
                role.Id,
                role.Name,
                role.Description))
            .ToList();
    }
}