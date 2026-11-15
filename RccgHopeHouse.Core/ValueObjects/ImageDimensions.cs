namespace RccgHopeHouse.Core.ValueObjects;

/// <summary>
/// Immutable Value Object representing image pixel dimensions.
/// Encapsulates width, height, and derived properties like aspect ratio and orientation.
/// Used for validation, responsive image rendering, and storage optimization decisions.
/// </summary>
/// <param name="Width">The image width in pixels (must be positive).</param>
/// <param name="Height">The image height in pixels (must be positive).</param>
public record ImageDimensions
{
    public int Width { get; init; }
    public int Height { get; init; }

    /// <summary>
    /// Validates that dimensions are positive and within reasonable bounds.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if width or height is &lt;= 0 or exceeds 10,000px.</exception>
    public ImageDimensions(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Image dimensions must be positive.", nameof(width));
        if (width > 10000 || height > 10000)
            throw new ArgumentException("Image dimensions exceed maximum allowed size (10,000px).", nameof(width));
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Calculates the aspect ratio as width divided by height.
    /// Useful for responsive layout decisions (e.g., 16:9 = 1.777, 4:3 = 1.333, 1:1 = 1.0).
    /// </summary>
    public double AspectRatio => Width / (double)Height;

    /// <summary>
    /// Total pixel count (width × height). Useful for estimating file size and processing time.
    /// </summary>
    public long TotalPixels => (long)Width * Height;

    /// <summary>
    /// Human-readable resolution string (e.g., "1920x1080").
    /// </summary>
    public string Resolution => $"{Width}x{Height}";

    /// <summary>
    /// Returns true if the image is wider than it is tall (landscape orientation).
    /// </summary>
    public bool IsLandscape => Width > Height;

    /// <summary>
    /// Returns true if the image is taller than it is wide (portrait orientation).
    /// </summary>
    public bool IsPortrait => Height > Width;

    /// <summary>
    /// Returns true if width equals height (square image).
    /// </summary>
    public bool IsSquare => Width == Height;

    /// <summary>
    /// Calculates new dimensions that fit within the specified max bounds while preserving aspect ratio.
    /// </summary>
    /// <param name="maxWidth">Maximum allowed width.</param>
    /// <param name="maxHeight">Maximum allowed height.</param>
    /// <returns>A new <see cref="ImageDimensions"/> instance scaled down if necessary.</returns>
    public ImageDimensions ResizeToFit(int maxWidth, int maxHeight)
    {
        if (Width <= maxWidth && Height <= maxHeight)
            return this; // Already fits

        var widthRatio = (double)maxWidth / Width;
        var heightRatio = (double)maxHeight / Height;
        var ratio = Math.Min(widthRatio, heightRatio);

        var newWidth = (int)(Width * ratio);
        var newHeight = (int)(Height * ratio);
        return new ImageDimensions(newWidth, newHeight);
    }

    /// <summary>
    /// Calculates dimensions for a square thumbnail crop centered on the original image.
    /// </summary>
    /// <param name="size">The target square size in pixels.</param>
    /// <returns>A new <see cref="ImageDimensions"/> with equal width and height.</returns>
    public ImageDimensions CropToSquare(int size) => new(size, size);

    /// <summary>
    /// Factory method to create a validated instance.
    /// </summary>
    /// <param name="width">Width in pixels.</param>
    /// <param name="height">Height in pixels.</param>
    /// <returns>A new <see cref="ImageDimensions"/> instance.</returns>
    /// <exception cref="ArgumentException">If dimensions are invalid.</exception>
    public static ImageDimensions Create(int width, int height) => new(width, height);

    /// <summary>
    /// Parses a resolution string (e.g., "1920x1080") into an <see cref="ImageDimensions"/> instance.
    /// </summary>
    /// <param name="resolution">String in "WxH" format.</param>
    /// <returns>Parsed dimensions, or null if format is invalid.</returns>
    public static ImageDimensions? Parse(string resolution)
    {
        if (string.IsNullOrWhiteSpace(resolution)) return null;

        var parts = resolution.Split('x', 'X');
        if (parts.Length != 2) return null;

        if (int.TryParse(parts[0], out var width) && int.TryParse(parts[1], out var height))
        {
            try { return new ImageDimensions(width, height); }
            catch { return null; }
        }
        return null;
    }
}