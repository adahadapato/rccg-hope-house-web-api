using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed class EnableAccountTwoFactorCommandHandler
    : IRequestHandler<
        EnableAccountTwoFactorCommand,
        IReadOnlyCollection<string>>
{
    private readonly IAccountService _accountService;

    public EnableAccountTwoFactorCommandHandler(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    public Task<IReadOnlyCollection<string>> Handle(
        EnableAccountTwoFactorCommand request,
        CancellationToken ct)
    {
        return _accountService.EnableTwoFactorAsync(
            request.UserId,
            request.VerificationCode,
            ct);
    }
}