using MediatR;
using RccgHopeHouse.Core.Interfaces.Admin;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Handles <see cref="UpdateAdminUserRolesCommand"/>.
/// </summary>
public sealed class UpdateAdminUserRolesCommandHandler
    : IRequestHandler<UpdateAdminUserRolesCommand>
{
    private readonly IAdminRepository _adminRepository;

    public UpdateAdminUserRolesCommandHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task Handle(
        UpdateAdminUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _adminRepository.GetUserByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException(
                $"User with ID '{request.UserId}' was not found.");
        }

        var currentlyAdmin =
            await _adminRepository.IsUserInRoleAsync(
                request.UserId,
                Core.Constants.Roles.Admin,
                cancellationToken);

        var willRemainAdmin = request.Roles.Any(
            role => string.Equals(
                role,
                Core.Constants.Roles.Admin,
                StringComparison.OrdinalIgnoreCase));

        if (currentlyAdmin && !willRemainAdmin)
        {
            if (string.Equals(
                request.UserId,
                request.CurrentUserId,
                StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "You cannot remove your own Admin role.");
            }

            if (user.IsActive)
            {
                var activeAdminCount =
                    await _adminRepository.CountActiveUsersInRoleAsync(
                        Core.Constants.Roles.Admin,
                        cancellationToken);

                if (activeAdminCount <= 1)
                {
                    throw new InvalidOperationException(
                        "The Admin role cannot be removed from the last active administrator.");
                }
            }
        }

        await _adminRepository.UpdateUserRolesAsync(
            request.UserId,
            request.Roles,
            cancellationToken);
    }
}
