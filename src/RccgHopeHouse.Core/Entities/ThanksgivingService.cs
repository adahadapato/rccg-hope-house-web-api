namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a monthly thanksgiving service video record.
/// Synced from YouTube playlist or manually entered by admin.
/// </summary>
public class ThanksgivingService : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string VideoUrl { get; private set; } = string.Empty;
    public string ThumbnailUrl { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime ServiceMonth { get; private set; }
    public DateTime PublishedAt { get; private set; }
    public bool IsAutoSynced { get; private set; }

    /// <summary>
    /// EF Core requires a parameterless constructor for materialization.
    /// </summary>
    private ThanksgivingService() { }

    /// <summary>
    /// Factory method to create a new thanksgiving service with validation.
    /// </summary>
    public static ThanksgivingService Create(
        string title,
        string videoUrl,
        DateTime publishedAt,
        string thumbnailUrl,
        string? description = null,
        bool isAutoSynced = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(videoUrl, nameof(videoUrl));
        ArgumentException.ThrowIfNullOrWhiteSpace(thumbnailUrl, nameof(thumbnailUrl));

        return new ThanksgivingService
        {
            Title = title.Trim(),
            VideoUrl = videoUrl.Trim(),
            ThumbnailUrl = thumbnailUrl.Trim(),
            Description = description?.Trim(),
            PublishedAt = publishedAt,
            ServiceMonth = new DateTime(publishedAt.Year, publishedAt.Month, 1), // Normalize to first of month
            IsAutoSynced = isAutoSynced
        };
    }

    /// <summary>
    /// Updates metadata for manually entered records.
    /// </summary>
    public void Update(string title, string? description, string thumbnailUrl)
    {
        Title = title.Trim();
        Description = description?.Trim();
        ThumbnailUrl = thumbnailUrl.Trim();
        MarkAsUpdated();
    }
}