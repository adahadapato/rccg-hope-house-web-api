using MediatR;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed class RemoveAccountProfileImageCommandHandler
    : IRequestHandler<
        RemoveAccountProfileImageCommand,
        AccountProfileResult>
{
    private readonly IAccountService _accountService;
    private readonly IGalleryImageStorage _imageStorage;

    public RemoveAccountProfileImageCommandHandler(
        IAccountService accountService,
        IGalleryImageStorage imageStorage)
    {
        _accountService = accountService;
        _imageStorage = imageStorage;
    }

    public async Task<AccountProfileResult> Handle(
        RemoveAccountProfileImageCommand request,
        CancellationToken ct)
    {
        var currentProfile =
            await _accountService.GetProfileAsync(
                request.UserId,
                ct);

        var updatedProfile =
            await _accountService.UpdateProfileImageAsync(
                request.UserId,
                null,
                ct);

        if (!string.IsNullOrWhiteSpace(
                currentProfile.ProfileImagePath))
        {
            try
            {
                await _imageStorage.DeleteAsync(
                    imagePath: null,
                    thumbnailPath:
                        currentProfile.ProfileImagePath,
                    cancellationToken: ct);
            }
            catch
            {
                // The database has already been updated.
                // A missing/orphaned physical file must not
                // make the profile update appear to have failed.
            }
        }

        return updatedProfile;
    }
}