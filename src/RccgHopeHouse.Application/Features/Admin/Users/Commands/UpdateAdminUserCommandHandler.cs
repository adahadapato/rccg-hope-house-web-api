using MediatR;
using RccgHopeHouse.Core.Interfaces.Admin;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Handles <see cref="UpdateAdminUserCommand"/>.
/// </summary>
public sealed class UpdateAdminUserCommandHandler
    : IRequestHandler<UpdateAdminUserCommand>
{
    private readonly IAdminRepository _adminRepository;

    public UpdateAdminUserCommandHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task Handle(
        UpdateAdminUserCommand request,
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

        var email = EmailAddress.Create(request.Email);

        var phoneNumber =
            PhoneNumber.CreateOrNull(request.PhoneNumber);

        var existingUser =
            await _adminRepository.GetUserByEmailAsync(
                email,
                cancellationToken);

        if (existingUser is not null &&
            !string.Equals(
                existingUser.Id,
                request.UserId,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Another user with this email address already exists.");
        }

        await _adminRepository.UpdateUserAsync(
            request.UserId,
            email,
            request.FirstName,
            request.LastName,
            phoneNumber,
            cancellationToken);
    }
}