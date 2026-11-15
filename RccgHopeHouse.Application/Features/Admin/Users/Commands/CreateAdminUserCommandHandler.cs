using MediatR;
using RccgHopeHouse.Core.Interfaces.Admin;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Handles <see cref="CreateAdminUserCommand"/>.
/// </summary>
public sealed class CreateAdminUserCommandHandler
    : IRequestHandler<CreateAdminUserCommand, string>
{
    private readonly IAdminRepository _adminRepository;

    public CreateAdminUserCommandHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<string> Handle(
        CreateAdminUserCommand request,
        CancellationToken cancellationToken)
    {
        var email = EmailAddress.Create(request.Email);

        var phoneNumber =
            PhoneNumber.CreateOrNull(request.PhoneNumber);

        var existingUser =
            await _adminRepository.GetUserByEmailAsync(
                email,
                cancellationToken);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "A user with this email address already exists.");
        }

        var userId =
            await _adminRepository.CreateUserAsync(
                email,
                request.FirstName,
                request.LastName,
                phoneNumber,
                request.Password,
                cancellationToken);

        if (request.Roles.Count > 0)
        {
            await _adminRepository.UpdateUserRolesAsync(
                userId,
                request.Roles,
                cancellationToken);
        }

        return userId;
    }
}
