using MediatR;
using RccgHopeHouse.Core.Interfaces.Admin;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Commands;

/// <summary>
/// Handles <see cref="UpdateAdminRoleCommand"/>.
/// </summary>
public sealed class UpdateAdminRoleCommandHandler
    : IRequestHandler<UpdateAdminRoleCommand>
{
    private readonly IAdminRepository _adminRepository;

    public UpdateAdminRoleCommandHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task Handle(
        UpdateAdminRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role =
            await _adminRepository.GetRoleByIdAsync(
                request.RoleId,
                cancellationToken);

        if (role is null)
        {
            throw new KeyNotFoundException(
                $"Role with ID '{request.RoleId}' was not found.");
        }

        var newRoleName =
            request.Name.Trim();

        var nameChanged =
            !string.Equals(
                role.Name,
                newRoleName,
                StringComparison.OrdinalIgnoreCase);

        if (nameChanged &&
            IsProtectedRole(role.Name))
        {
            throw new InvalidOperationException(
                $"The system role '{role.Name}' cannot be renamed.");
        }

        if (nameChanged)
        {
            var existingRole =
                await _adminRepository.GetRoleByNameAsync(
                    newRoleName,
                    cancellationToken);

            if (existingRole is not null &&
                !string.Equals(
                    existingRole.Id,
                    request.RoleId,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"A role named '{newRoleName}' already exists.");
            }
        }

        await _adminRepository.UpdateRoleAsync(
            request.RoleId,
            newRoleName,
            request.Description,
            cancellationToken);
    }

    private static bool IsProtectedRole(
        string roleName) =>
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