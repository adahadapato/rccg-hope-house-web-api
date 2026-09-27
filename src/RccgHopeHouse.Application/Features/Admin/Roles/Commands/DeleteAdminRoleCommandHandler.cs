using MediatR;
using RccgHopeHouse.Core.Interfaces.Admin;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Commands;

/// <summary>
/// Handles <see cref="DeleteAdminRoleCommand"/>.
/// </summary>
public sealed class DeleteAdminRoleCommandHandler
    : IRequestHandler<DeleteAdminRoleCommand>
{
    private readonly IAdminRepository _adminRepository;

    public DeleteAdminRoleCommandHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task Handle(
        DeleteAdminRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await _adminRepository.GetRoleByIdAsync(
            request.RoleId,
            cancellationToken);

        if (role is null)
        {
            throw new KeyNotFoundException(
                $"Role with ID '{request.RoleId}' was not found.");
        }

        if (IsProtectedRole(role.Name))
        {
            throw new InvalidOperationException(
                $"The system role '{role.Name}' cannot be deleted.");
        }

        var assignedUserCount =
            await _adminRepository.CountUsersInRoleAsync(
                role.Name,
                cancellationToken);

        if (assignedUserCount > 0)
        {
            throw new InvalidOperationException(
                $"The role '{role.Name}' cannot be deleted because " +
                $"{assignedUserCount} user(s) are currently assigned to it.");
        }

        await _adminRepository.DeleteRoleAsync(
            request.RoleId,
            cancellationToken);
    }

    private static bool IsProtectedRole(string roleName) =>
        string.Equals(
            roleName,
            Core.Constants.Roles.Admin,
            StringComparison.OrdinalIgnoreCase)
        ||
        string.Equals(
            roleName,
            Core.Constants.Roles.ContentEditor,
            StringComparison.OrdinalIgnoreCase)
        ||
        string.Equals(
            roleName,
            Core.Constants.Roles.MediaManager,
            StringComparison.OrdinalIgnoreCase)
        ||
        string.Equals(
            roleName,
            Core.Constants.Roles.PrayerTeam,
            StringComparison.OrdinalIgnoreCase);
}
