using MediatR;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Commands;

/// <summary>
/// Handles <see cref="UpdateAccountPhoneNumberCommand"/>.
/// </summary>
public sealed class UpdateAccountPhoneNumberCommandHandler
    : IRequestHandler<
        UpdateAccountPhoneNumberCommand,
        AccountProfileResult>
{
    private readonly IAccountService _accountService;

    public UpdateAccountPhoneNumberCommandHandler(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<AccountProfileResult> Handle(
        UpdateAccountPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        return await _accountService.UpdatePhoneNumberAsync(
            request.UserId,
            request.PhoneNumber,
            cancellationToken);
    }
}