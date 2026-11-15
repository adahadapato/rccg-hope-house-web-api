using MediatR;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Commands;

/// <summary>
/// Handles <see cref="UpdateAccountProfileCommand"/>.
/// </summary>
public sealed class UpdateAccountProfileCommandHandler
    : IRequestHandler<
        UpdateAccountProfileCommand,
        AccountProfileResult>
{
    private readonly IAccountService _accountService;

    public UpdateAccountProfileCommandHandler(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<AccountProfileResult> Handle(
        UpdateAccountProfileCommand request,
        CancellationToken cancellationToken)
    {
        return await _accountService.UpdateProfileAsync(
            request.UserId,
            request.FirstName,
            request.LastName,
            cancellationToken);
    }
}