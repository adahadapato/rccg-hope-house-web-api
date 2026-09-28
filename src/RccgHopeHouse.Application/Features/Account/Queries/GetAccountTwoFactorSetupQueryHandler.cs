using MediatR;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Queries;

public sealed class GetAccountTwoFactorSetupQueryHandler
    : IRequestHandler<
        GetAccountTwoFactorSetupQuery,
        TwoFactorSetupResult>
{
    private readonly IAccountService _accountService;

    public GetAccountTwoFactorSetupQueryHandler(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    public Task<TwoFactorSetupResult> Handle(
        GetAccountTwoFactorSetupQuery request,
        CancellationToken ct)
    {
        return _accountService.GetTwoFactorSetupAsync(
            request.UserId,
            ct);
    }
}