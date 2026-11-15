using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed class DisableAccountTwoFactorCommandHandler
    : IRequestHandler<
        DisableAccountTwoFactorCommand>
{
    private readonly IAccountService _accountService;

    public DisableAccountTwoFactorCommandHandler(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task Handle(
        DisableAccountTwoFactorCommand request,
        CancellationToken ct)
    {
        await _accountService.DisableTwoFactorAsync(
            request.UserId,
            ct);
    }
}