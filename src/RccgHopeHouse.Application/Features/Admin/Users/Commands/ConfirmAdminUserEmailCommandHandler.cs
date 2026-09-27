using MediatR;
using RccgHopeHouse.Core.Interfaces.Admin;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Handles <see cref="ConfirmAdminUserEmailCommand"/>.
/// </summary>
public sealed class ConfirmAdminUserEmailCommandHandler
    : IRequestHandler<ConfirmAdminUserEmailCommand>
{
    private readonly IAdminRepository _adminRepository;

    public ConfirmAdminUserEmailCommandHandler(
        IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task Handle(
        ConfirmAdminUserEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _adminRepository.GetUserByIdAsync(
                request.UserId,
                cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException(
                $"User with ID '{request.UserId}' was not found.");
        }

        if (user.EmailConfirmed)
        {
            return;
        }

        await _adminRepository.ConfirmEmailAsync(
            request.UserId,
            request.Token,
            cancellationToken);
    }
}
