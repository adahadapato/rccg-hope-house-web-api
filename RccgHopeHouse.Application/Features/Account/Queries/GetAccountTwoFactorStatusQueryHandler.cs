using MediatR;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Queries;

public sealed class GetAccountTwoFactorStatusQueryHandler
    : IRequestHandler<
        GetAccountTwoFactorStatusQuery,
        TwoFactorStatusResult>
{
    private readonly IAccountService _accountService;

    public GetAccountTwoFactorStatusQueryHandler(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    public Task<TwoFactorStatusResult> Handle(
        GetAccountTwoFactorStatusQuery request,
        CancellationToken ct)
    {
        return _accountService.GetTwoFactorStatusAsync(
            request.UserId,
            ct);
    }
}