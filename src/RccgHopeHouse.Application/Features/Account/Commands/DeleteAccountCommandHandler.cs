using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed class DeleteAccountCommandHandler
    : IRequestHandler<DeleteAccountCommand>
{
    private readonly IAccountService _accountService;
    private readonly IGalleryImageStorage _imageStorage;

    public DeleteAccountCommandHandler(
        IAccountService accountService,
        IGalleryImageStorage imageStorage)
    {
        _accountService = accountService;
        _imageStorage = imageStorage;
    }

    public async Task Handle(
        DeleteAccountCommand request,
        CancellationToken ct)
    {
        var profile =
            await _accountService.GetProfileAsync(
                request.UserId,
                ct);

        await _accountService.DeleteAccountAsync(
            request.UserId,
            request.CurrentPassword,
            ct);

        if (!string.IsNullOrWhiteSpace(
                profile.ProfileImagePath))
        {
            try
            {
                await _imageStorage.DeleteAsync(
                    imagePath: null,
                    thumbnailPath:
                        profile.ProfileImagePath,
                    cancellationToken: ct);
            }
            catch
            {
                // The account has already been deleted.
                // Failure to clean up an orphaned profile
                // image must not make account deletion
                // appear to have failed.
            }
        }
    }
}