using MediatR;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Queries;

/// <summary>
/// Handles <see cref="GetAccountProfileQuery"/>.
/// </summary>
public sealed class GetAccountProfileQueryHandler
    : IRequestHandler<GetAccountProfileQuery, AccountProfileResult>
{
    private readonly IAccountService _accountService;

    public GetAccountProfileQueryHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<AccountProfileResult> Handle(GetAccountProfileQuery request, CancellationToken cancellationToken)
    {
        return await _accountService.GetProfileAsync(request.UserId, cancellationToken);
    }
}