using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed class GenerateAccountRecoveryCodesCommandHandler
    : IRequestHandler<
        GenerateAccountRecoveryCodesCommand,
        IReadOnlyCollection<string>>
{
    private readonly IAccountService _accountService;

    public GenerateAccountRecoveryCodesCommandHandler(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    public Task<IReadOnlyCollection<string>> Handle(
        GenerateAccountRecoveryCodesCommand request,
        CancellationToken ct)
    {
        return _accountService.GenerateRecoveryCodesAsync(
            request.UserId,
            ct);
    }
}