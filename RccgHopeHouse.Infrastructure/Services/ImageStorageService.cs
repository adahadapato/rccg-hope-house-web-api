using ImageMagick;
using Microsoft.Extensions.Configuration;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IImageStorageService"/> using Magick.NET for server-side compression.
/// Free for commercial use under MIT license. Supports 100+ image formats.
/// </summary>
public class ImageStorageService : IImageStorageService
{
    private readonly IConfiguration _configuration;

    public ImageStorageService(IConfiguration configuration) => _configuration = configuration;

    /// <inheritdoc />
    public async Task<ImageUploadResult> UploadAsync(
        Stream fileStream, string fileName, string contentType,
        ImageUploadOptions? options = null, CancellationToken ct = default)
    {
        options ??= new ImageUploadOptions();

        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms, ct);
        ms.Position = 0;

        using var image = new MagickImage(ms);

        if (options.MaxWidth.HasValue || options.MaxHeight.HasValue)
        {
            var width = options.MaxWidth.HasValue ? (uint)options.MaxWidth.Value : image.Width;
            var height = options.MaxHeight.HasValue ? (uint)options.MaxHeight.Value : image.Height;
            image.Resize(new MagickGeometry(width, height)
            {
                IgnoreAspectRatio = false
            });
        }

        image.Format = MagickFormat.Jpeg;
        image.Quality = (uint)options.Quality;

        using var mainStream = new MemoryStream();
        await image.WriteAsync(mainStream, ct);
        var mainBytes = mainStream.ToArray();

        byte[]? thumbnailBytes = null;
        if (options.GenerateThumbnail)
        {
            using var thumbImage = image.Clone();
            thumbImage.Crop(
                (uint)(options.ThumbnailWidth != 0 ? options.ThumbnailWidth : 300),
                (uint)(options.ThumbnailHeight != 0 ? options.ThumbnailHeight : 300),
                Gravity.Center);
            thumbImage.Quality = 75;
            thumbImage.Format = MagickFormat.Jpeg;

            using var thumbStream = new MemoryStream();
            await thumbImage.WriteAsync(thumbStream, ct);
            thumbnailBytes = thumbStream.ToArray();
        }

        return new ImageUploadResult(
            ImageUrl: $"db://{fileName}",
            ThumbnailUrl: options.GenerateThumbnail ? $"db://{fileName}_thumb" : null,
            Width: (int)image.Width,
            Height: (int)image.Height,
            FileSizeBytes: mainBytes.Length,
            ContentType: "image/jpeg"
        );
    }

    /// <inheritdoc />
    public async Task<ImageDimensions?> GetDimensionsAsync(string imageUrl, CancellationToken ct = default)
    {
        if (imageUrl.StartsWith("db://", StringComparison.OrdinalIgnoreCase))
            return null;

        if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            return null;

        try
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(imageUrl, HttpCompletionOption.ResponseHeadersRead, ct);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(ct);
            var imageInfo = new MagickImageInfo(stream);

            return new ImageDimensions((int)imageInfo.Width, (int)imageInfo.Height);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(string imageUrl, CancellationToken ct = default) =>
        Task.FromResult(true);

    /// <inheritdoc />
    public Task<string> GetPublicUrlAsync(string imagePath, CancellationToken ct = default) =>
        Task.FromResult(imagePath);
}