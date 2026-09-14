namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Result record for image processing operations.
/// Returns optimized metadata without leaking provider-specific types.
/// Used by Application handlers to persist binary data and track dimensions.
/// </summary>
public record ImageUploadResult(
    string ImageUrl,
    string? ThumbnailUrl,
    int Width,
    int Height,
    long FileSizeBytes,
    string ContentType);

/// <summary>
/// Configuration options for image processing and optimization.
/// Allows callers to control resizing, thumbnail generation, and compression quality.
/// </summary>
public record ImageUploadOptions
{
    /// <summary>
    /// If true, generates a cropped thumbnail for gallery grid views.
    /// </summary>
    public bool GenerateThumbnail { get; set; } = true;

    /// <summary>
    /// Maximum width for the main image. Maintains aspect ratio.
    /// </summary>
    public int? MaxWidth { get; set; } = 1920;

    /// <summary>
    /// Maximum height for the main image. Maintains aspect ratio.
    /// </summary>
    public int? MaxHeight { get; set; } = 1080;

    /// <summary>
    /// Thumbnail width in pixels (used with ThumbnailHeight for square crop).
    /// </summary>
    public int ThumbnailWidth { get; set; } = 300;

    /// <summary>
    /// Thumbnail height in pixels.
    /// </summary>
    public int ThumbnailHeight { get; set; } = 300;

    /// <summary>
    /// JPEG compression quality (1-100). Default 85 balances size and visual fidelity.
    /// </summary>
    public int Quality { get; set; } = 85;
}

/// <summary>
/// Abstraction for image processing, optimization, and storage preparation.
/// Implemented in Infrastructure using ImageSharp for server-side compression.
/// Keeps Application layer decoupled from image manipulation libraries.
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Processes an uploaded image stream: resizes, compresses, and optionally generates a thumbnail.
    /// Returns optimized metadata for database persistence. Does NOT store externally; returns byte[]-ready result.
    /// </summary>
    /// <param name="imageStream">The raw uploaded image stream.</param>
    /// <param name="fileName">Original file name (used for logging/metadata).</param>
    /// <param name="originalContentType">MIME type of the uploaded file.</param>
    /// <param name="options">Processing configuration. Defaults to 1920x1080 max, 300x300 thumb, 85% quality.</param>
    /// <param name="ct">Cancellation token for async operations.</param>
    /// <returns>ImageUploadResult containing optimized dimensions, size, and placeholder URLs.</returns>
    /// <remarks>
    /// This method is CPU-bound. Consider running in a background queue for high-traffic upload endpoints.
    /// Returns JPEG-optimized streams regardless of input format for consistent DB storage.
    /// </remarks>
    Task<ImageUploadResult> UploadAsync(
        Stream imageStream,
        string fileName,
        string originalContentType,
        ImageUploadOptions? options = null,
        CancellationToken ct = default);
}