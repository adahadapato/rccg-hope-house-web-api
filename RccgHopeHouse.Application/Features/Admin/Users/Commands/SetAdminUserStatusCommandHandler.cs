using MediatR;
using RccgHopeHouse.Core.Interfaces.Admin;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Handles <see cref="SetAdminUserStatusCommand"/>.
/// </summary>
public sealed class SetAdminUserStatusCommandHandler
    : IRequestHandler<SetAdminUserStatusCommand>
{
    private readonly IAdminRepository _adminRepository;

    public SetAdminUserStatusCommandHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task Handle(
        SetAdminUserStatusCommand request,
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

        if (user.IsActive == request.IsActive)
            return;

        if (!request.IsActive)
        {
            if (string.Equals(
                request.UserId,
                request.CurrentUserId,
                StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "You cannot deactivate your own account.");
            }

            var isAdmin =
                await _adminRepository.IsUserInRoleAsync(
                    request.UserId,
                    Core.Constants.Roles.Admin,
                    cancellationToken);

            if (isAdmin)
            {
                var activeAdminCount =
                    await _adminRepository.CountActiveUsersInRoleAsync(
                        Core.Constants.Roles.Admin,
                        cancellationToken);

                if (activeAdminCount <= 1)
                {
                    throw new InvalidOperationException(
                        "The last active administrator cannot be deactivated.");
                }
            }
        }

        await _adminRepository.SetUserActiveStatusAsync(
            request.UserId,
            request.IsActive,
            cancellationToken);
    }
}
