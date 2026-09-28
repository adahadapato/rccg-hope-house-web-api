using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Account.Commands;

/// <summary>
/// Handles <see cref="ChangeAccountPasswordCommand"/>.
/// </summary>
public sealed class ChangeAccountPasswordCommandHandler
    : IRequestHandler<ChangeAccountPasswordCommand>
{
    private readonly IAccountService _accountService;

    public ChangeAccountPasswordCommandHandler(
        IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task Handle(
        ChangeAccountPasswordCommand request,
        CancellationToken cancellationToken)
    {
        await _accountService.ChangePasswordAsync(
            request.UserId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);
    }
}