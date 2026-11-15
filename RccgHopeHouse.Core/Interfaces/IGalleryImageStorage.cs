using RccgHopeHouse.Core.Models;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Abstraction for storing and deleting physical gallery image files.
///
/// Application and Core code do not need to know whether images
/// are stored on the local file system, Azure Blob Storage,
/// Amazon S3, or another storage provider.
/// </summary>
public interface IGalleryImageStorage
{
    /// <summary>
    /// Processes and stores a newly uploaded gallery image.
    /// </summary>
    /// <param name="imageData">
    /// Original uploaded image bytes.
    /// </param>
    /// <param name="contentType">
    /// MIME type supplied with the upload.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Information about the stored full-size image and thumbnail.
    /// </returns>
    Task<GalleryStoredImage> SaveAsync(
        byte[] imageData,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a full-size image and its thumbnail from storage.
    /// Missing files are ignored.
    /// </summary>
    Task DeleteAsync(
        string? imagePath,
        string? thumbnailPath,
        CancellationToken cancellationToken = default);
}