using MediatR;
using RccgHopeHouse.Application.Features.Admin.Users.Dtos;
using RccgHopeHouse.Core.Interfaces.Admin;

namespace RccgHopeHouse.Application.Features.Admin.Users.Queries;

/// <summary>
/// Handles <see cref="GetAdminUsersQuery"/>.
/// </summary>
public sealed class GetAdminUsersQueryHandler
    : IRequestHandler<GetAdminUsersQuery, IReadOnlyList<AdminUserDto>>
{
    private readonly IAdminRepository _adminRepository;

    public GetAdminUsersQueryHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<IReadOnlyList<AdminUserDto>> Handle(
        GetAdminUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _adminRepository.GetUsersAsync(
            cancellationToken);

        return users
            .Select(user => new AdminUserDto(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.EmailConfirmed,
                user.TwoFactorEnabled,
                user.IsActive,
                user.LastLoginAt,
                user.Roles))
            .ToList();
    }
}
