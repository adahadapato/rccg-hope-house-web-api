using System.Security.Cryptography;
using ImageMagick;
using Microsoft.Extensions.Configuration;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Models;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Stores gallery images on the web application's physical
/// file system.
///
/// Images are written beneath:
///
/// wwwroot/uploads/gallery
///
/// Full-size images are optimised and stored as WebP.
/// A smaller WebP thumbnail is generated automatically.
///
/// SQL Server stores only the resulting public paths and
/// image metadata.
/// </summary>
public sealed class FileSystemGalleryImageStorageService
    : IGalleryImageStorage
{
    private const int MaximumUploadBytes =
        10 * 1024 * 1024;

    private const uint MaximumImageDimension =
        2560;

    private const uint ThumbnailWidth =
        600;

    private const uint ThumbnailHeight =
        600;

    private const uint FullImageQuality =
        82;

    private const uint ThumbnailQuality =
        78;

    private readonly string _webRootPath;

    public FileSystemGalleryImageStorageService(
        IConfiguration configuration)
    {
        var configuredWebRoot =
            configuration[
                "GalleryStorage:WebRootPath"];

        _webRootPath =
            ResolveWebRootPath(
                configuredWebRoot);

        var galleryRootPath =
            Path.Combine(
                _webRootPath,
                "uploads",
                "gallery");

        Directory.CreateDirectory(
            galleryRootPath);
    }

    /// <inheritdoc />
    public async Task<GalleryStoredImage> SaveAsync(
        byte[] imageData,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            imageData);

        if (imageData.Length == 0)
        {
            throw new ArgumentException(
                "Image data cannot be empty.",
                nameof(imageData));
        }

        if (imageData.Length >
            MaximumUploadBytes)
        {
            throw new ArgumentException(
                "Image size cannot exceed 10 MB.",
                nameof(imageData));
        }

        cancellationToken
            .ThrowIfCancellationRequested();

        ValidateDeclaredContentType(
            contentType);

        var imageHash =
            GenerateHash(
                imageData);

        using var image =
            new MagickImage(
                imageData);

        ValidateActualImage(
            image);

        // Correct camera/phone orientation while the
        // EXIF orientation information still exists.
        image.AutoOrient();

        // Remove EXIF, GPS and other metadata before
        // publishing the photograph.
        image.Strip();

        ResizeFullImageIfRequired(
            image);

        var width =
            checked(
                (int)image.Width);

        var height =
            checked(
                (int)image.Height);

        var now =
            DateTime.UtcNow;

        var year =
            now.Year.ToString("0000");

        var month =
            now.Month.ToString("00");

        var relativeDirectory =
            Path.Combine(
                "uploads",
                "gallery",
                year,
                month);

        var physicalDirectory =
            Path.Combine(
                _webRootPath,
                relativeDirectory);

        var thumbnailDirectory =
            Path.Combine(
                physicalDirectory,
                "thumbnails");

        Directory.CreateDirectory(
            physicalDirectory);

        Directory.CreateDirectory(
            thumbnailDirectory);

        var fileName =
            $"{Guid.NewGuid():N}.webp";

        var physicalImagePath =
            Path.Combine(
                physicalDirectory,
                fileName);

        var physicalThumbnailPath =
            Path.Combine(
                thumbnailDirectory,
                fileName);

        try
        {
            ConfigureFullImage(
                image);

            await image.WriteAsync(
                physicalImagePath,
                cancellationToken);

            cancellationToken
                .ThrowIfCancellationRequested();

            using var thumbnail =
                new MagickImage(
                    physicalImagePath);

            ConfigureThumbnail(
                thumbnail);

            await thumbnail.WriteAsync(
                physicalThumbnailPath,
                cancellationToken);

            var storedFileSize =
                new FileInfo(
                    physicalImagePath)
                .Length;

            if (storedFileSize >
                int.MaxValue)
            {
                throw new InvalidOperationException(
                    "Stored gallery image is too large.");
            }

            var publicImagePath =
                ToPublicPath(
                    relativeDirectory,
                    fileName);

            var publicThumbnailPath =
                ToPublicPath(
                    relativeDirectory,
                    "thumbnails",
                    fileName);

            return new GalleryStoredImage(
                ImagePath:
                    publicImagePath,
                ThumbnailPath:
                    publicThumbnailPath,
                ContentType:
                    "image/webp",
                FileSizeBytes:
                    (int)storedFileSize,
                Width:
                    width,
                Height:
                    height,
                ImageHash:
                    imageHash);
        }
        catch
        {
            TryDeleteFile(
                physicalImagePath);

            TryDeleteFile(
                physicalThumbnailPath);

            throw;
        }
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        string? imagePath,
        string? thumbnailPath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken
            .ThrowIfCancellationRequested();

        DeletePublicFile(
            imagePath);

        DeletePublicFile(
            thumbnailPath);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Resolves the application's web root.
    ///
    /// Program.cs supplies ASP.NET Core's actual WebRootPath
    /// through GalleryStorage:WebRootPath.
    ///
    /// The fallback is retained for non-web hosts such as
    /// tests or background processes.
    /// </summary>
    private static string ResolveWebRootPath(
        string? configuredWebRoot)
    {
        if (!string.IsNullOrWhiteSpace(
                configuredWebRoot))
        {
            return Path.GetFullPath(
                configuredWebRoot);
        }

        var currentDirectoryWebRoot =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot");

        if (Directory.Exists(
                currentDirectoryWebRoot))
        {
            return Path.GetFullPath(
                currentDirectoryWebRoot);
        }

        return Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "wwwroot"));
    }

    /// <summary>
    /// Restricts MIME types accepted by the gallery.
    ///
    /// The image bytes are also decoded by Magick.NET,
    /// so the browser-provided MIME type is not trusted
    /// by itself.
    /// </summary>
    private static void ValidateDeclaredContentType(
        string contentType)
    {
        if (string.IsNullOrWhiteSpace(
                contentType))
        {
            throw new ArgumentException(
                "Image content type is required.",
                nameof(contentType));
        }

        var allowedContentTypes =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

        if (!allowedContentTypes.Contains(
                contentType.Trim()))
        {
            throw new ArgumentException(
                "Only JPEG, PNG and WebP images are supported.",
                nameof(contentType));
        }
    }

    /// <summary>
    /// Ensures the uploaded bytes represent a supported
    /// image rather than trusting the browser MIME type.
    /// </summary>
    private static void ValidateActualImage(
        MagickImage image)
    {
        if (image.Width == 0 ||
            image.Height == 0)
        {
            throw new ArgumentException(
                "The uploaded file is not a valid image.");
        }

        var supportedFormats =
            new[]
            {
                MagickFormat.Jpeg,
                MagickFormat.Jpg,
                MagickFormat.Png,
                MagickFormat.WebP
            };

        if (!supportedFormats.Contains(
                image.Format))
        {
            throw new ArgumentException(
                "Only JPEG, PNG and WebP images are supported.");
        }
    }

    /// <summary>
    /// Prevents unnecessarily large camera images from
    /// being served directly to website visitors.
    ///
    /// Aspect ratio is preserved and smaller images are
    /// never enlarged.
    /// </summary>
    private static void ResizeFullImageIfRequired(
        MagickImage image)
    {
        if (image.Width <=
                MaximumImageDimension &&
            image.Height <=
                MaximumImageDimension)
        {
            return;
        }

        var geometry =
            new MagickGeometry(
                MaximumImageDimension,
                MaximumImageDimension)
            {
                IgnoreAspectRatio = false,
                Greater = true
            };

        image.Resize(
            geometry);
    }

    /// <summary>
    /// Configures the full-size public image.
    /// </summary>
    private static void ConfigureFullImage(
        MagickImage image)
    {
        image.Format =
            MagickFormat.WebP;

        image.Quality =
            FullImageQuality;
    }

    /// <summary>
    /// Creates a lightweight thumbnail while preserving
    /// the original aspect ratio.
    /// </summary>
    private static void ConfigureThumbnail(
        MagickImage image)
    {
        var geometry =
            new MagickGeometry(
                ThumbnailWidth,
                ThumbnailHeight)
            {
                IgnoreAspectRatio = false,
                Greater = true
            };

        image.Resize(
            geometry);

        image.Strip();

        image.Format =
            MagickFormat.WebP;

        image.Quality =
            ThumbnailQuality;
    }

    /// <summary>
    /// Calculates SHA256 from the original upload.
    ///
    /// This can later be used for duplicate detection.
    /// </summary>
    private static string GenerateHash(
        byte[] imageData)
    {
        var hash =
            SHA256.HashData(
                imageData);

        return Convert.ToHexString(
            hash);
    }

    /// <summary>
    /// Converts physical path segments into an
    /// application-relative browser URL.
    /// </summary>
    private static string ToPublicPath(
        params string[] segments)
    {
        var path =
            string.Join(
                "/",
                segments.Select(
                    segment =>
                        segment
                            .Replace('\\', '/')
                            .Trim('/')));

        return "/" + path;
    }

    /// <summary>
    /// Deletes a stored gallery file using its
    /// application-relative public path.
    /// </summary>
    private void DeletePublicFile(
        string? publicPath)
    {
        if (string.IsNullOrWhiteSpace(
                publicPath))
        {
            return;
        }

        var relativePath =
            publicPath
                .Trim()
                .TrimStart('/')
                .Replace(
                    '/',
                    Path.DirectorySeparatorChar);

        var physicalPath =
            Path.GetFullPath(
                Path.Combine(
                    _webRootPath,
                    relativePath));

        var fullWebRoot =
            Path.GetFullPath(
                _webRootPath);

        var rootWithSeparator =
            fullWebRoot.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        // Prevent a malformed or manipulated database path
        // from escaping the configured web-root directory.
        if (!physicalPath.StartsWith(
                rootWithSeparator,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Gallery image path is outside the configured web root.");
        }

        TryDeleteFile(
            physicalPath);
    }

    /// <summary>
    /// Deletes a file when present.
    /// Missing files are intentionally ignored.
    /// </summary>
    private static void TryDeleteFile(
        string path)
    {
        if (File.Exists(
                path))
        {
            File.Delete(
                path);
        }
    }
}