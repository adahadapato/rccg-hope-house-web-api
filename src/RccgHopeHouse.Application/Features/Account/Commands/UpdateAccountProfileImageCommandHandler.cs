using MediatR;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed class UpdateAccountProfileImageCommandHandler
    : IRequestHandler<
        UpdateAccountProfileImageCommand,
        AccountProfileResult>
{
    private readonly IAccountService _accountService;
    private readonly IGalleryImageStorage _imageStorage;

    public UpdateAccountProfileImageCommandHandler(
        IAccountService accountService,
        IGalleryImageStorage imageStorage)
    {
        _accountService = accountService;
        _imageStorage = imageStorage;
    }

    public async Task<AccountProfileResult> Handle(
        UpdateAccountProfileImageCommand request,
        CancellationToken ct)
    {
        var currentProfile =
            await _accountService.GetProfileAsync(
                request.UserId,
                ct);

        var storedImage =
            await _imageStorage.SaveAsync(
                request.ImageData,
                request.ContentType,
                ct);

        try
        {
            // Use the generated thumbnail for the admin
            // profile/avatar image.
            var profileImagePath =
                storedImage.ThumbnailPath
                ?? storedImage.ImagePath;

            var updatedProfile =
                await _accountService.UpdateProfileImageAsync(
                    request.UserId,
                    profileImagePath,
                    ct);

            // Only remove the previous image after the
            // database update has succeeded.
            if (!string.IsNullOrWhiteSpace(
                    currentProfile.ProfileImagePath))
            {
                await _imageStorage.DeleteAsync(
                    imagePath: null,
                    thumbnailPath:
                        currentProfile.ProfileImagePath,
                    cancellationToken: ct);
            }

            // We only retain the thumbnail for the profile.
            // The larger gallery-style image is unnecessary.
            if (!string.Equals(
                    profileImagePath,
                    storedImage.ImagePath,
                    StringComparison.OrdinalIgnoreCase))
            {
                await _imageStorage.DeleteAsync(
                    imagePath:
                        storedImage.ImagePath,
                    thumbnailPath: null,
                    cancellationToken: ct);
            }

            return updatedProfile;
        }
        catch
        {
            try
            {
                await _imageStorage.DeleteAsync(
                    storedImage.ImagePath,
                    storedImage.ThumbnailPath,
                    CancellationToken.None);
            }
            catch
            {
                // Preserve the original exception.
            }

            throw;
        }
    }
}