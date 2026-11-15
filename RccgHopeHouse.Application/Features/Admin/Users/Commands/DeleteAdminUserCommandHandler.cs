using MediatR;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces.Admin;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

public sealed class DeleteAdminUserCommandHandler
 : IRequestHandler<DeleteAdminUserCommand>
{
    private readonly IAdminRepository _adminRepository;

    public DeleteAdminUserCommandHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task Handle(
        DeleteAdminUserCommand request,
        CancellationToken cancellationToken)
    {
        // An administrator must never be able
        // to delete their own account.
        if (string.Equals(
                request.UserId,
                request.CurrentUserId,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException(
                "You cannot delete your own account.");
        }

        var user =
            await _adminRepository.GetUserByIdAsync(
                request.UserId,
                cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", request.UserId);
        }

        var isAdmin =
            await _adminRepository.IsUserInRoleAsync(
                request.UserId,
                Core.Constants.Roles.Admin,
                cancellationToken);

        if (isAdmin && user.IsActive)
        {
            var activeAdminCount =
                await _adminRepository.CountActiveUsersInRoleAsync(
                    Core.Constants.Roles.Admin,
                    cancellationToken);

            if (activeAdminCount <= 1)
            {
                throw new DomainException(
                    "The last active administrator cannot be deleted.");
            }
        }

        await _adminRepository.DeleteUserAsync(
            request.UserId,
            cancellationToken);
    }
}
