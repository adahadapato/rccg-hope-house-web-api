using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a YouTube broadcast associated with a church service
/// for a particular service month.
/// </summary>
public class ServiceBroadcast : BaseEntity
{
    /// <summary>
    /// Retained for compatibility and useful classification,
    /// but ChurchServiceId is the authoritative relationship
    /// between a broadcast and a particular service.
    /// </summary>
    public ServiceCategory Category { get; private set; }

    /// <summary>
    /// Gets the identifier of the church service associated
    /// with this broadcast.
    /// </summary>
    public Guid ChurchServiceId { get; private set; }

    /// <summary>
    /// Gets the church service associated with this broadcast.
    /// </summary>
    public ChurchService ChurchService { get; private set; } = null!;

    /// <summary>
    /// Gets the broadcast title.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the YouTube video identifier.
    /// </summary>
    public string VideoId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the broadcast description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the month associated with the broadcast.
    /// The value is normalized to the first day of the month.
    /// </summary>
    public DateTime ServiceMonth { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the broadcast is currently live.
    /// </summary>
    public bool IsLive { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the broadcast should be shown
    /// on the public website.
    /// </summary>
    public bool IsPublished { get; private set; }

    /// <summary>
    /// Gets this month's sermon or service theme badge.
    /// </summary>
    public string? Theme { get; private set; }

    /// <summary>
    /// Gets the YouTube thumbnail URL for the broadcast.
    /// </summary>
    public string ThumbnailUrl =>
        $"https://i.ytimg.com/vi/{VideoId}/hqdefault.jpg";

    /// <summary>
    /// Gets the YouTube watch URL for the broadcast.
    /// </summary>
    public string VideoUrl =>
        $"https://www.youtube.com/watch?v={VideoId}";

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ServiceBroadcast"/> class for Entity Framework.
    /// </summary>
    private ServiceBroadcast()
    {
    }

    /// <summary>
    /// Creates a new service broadcast.
    /// </summary>
    /// <param name="churchServiceId">
    /// Identifier of the church service associated with the broadcast.
    /// </param>
    /// <param name="category">
    /// Category of the associated church service.
    /// </param>
    /// <param name="title">
    /// Broadcast title.
    /// </param>
    /// <param name="youtubeUrl">
    /// YouTube URL or video identifier.
    /// </param>
    /// <param name="serviceMonth">
    /// Month associated with the broadcast.
    /// </param>
    /// <param name="description">
    /// Optional broadcast description.
    /// </param>
    /// <param name="theme">
    /// Optional service theme.
    /// </param>
    /// <param name="isLive">
    /// Indicates whether the broadcast is currently live.
    /// </param>
    /// <param name="isPublished">
    /// Indicates whether the broadcast should be shown publicly.
    /// </param>
    /// <returns>
    /// A new service broadcast.
    /// </returns>
    public static ServiceBroadcast Create(
        Guid churchServiceId,
        ServiceCategory category,
        string title,
        string youtubeUrl,
        DateTime serviceMonth,
        string? description = null,
        string? theme = null,
        bool isLive = false,
        bool isPublished = true)
    {
        if (churchServiceId == Guid.Empty)
        {
            throw new ArgumentException(
                "A church service is required.",
                nameof(churchServiceId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            title,
            nameof(title));

        var videoId =
            ExtractVideoId(youtubeUrl);

        return new ServiceBroadcast
        {
            ChurchServiceId = churchServiceId,
            Category = category,
            Title = title.Trim(),
            VideoId = videoId,
            Description =
                NormalizeOptionalText(description),
            Theme =
                NormalizeOptionalText(theme),
            ServiceMonth = new DateTime(
                serviceMonth.Year,
                serviceMonth.Month,
                1),
            IsLive = isLive,
            IsPublished = isPublished
        };
    }

    /// <summary>
    /// Updates the broadcast content and live status.
    /// Publication status is intentionally not changed by this method.
    /// </summary>
    /// <param name="title">
    /// Updated broadcast title.
    /// </param>
    /// <param name="youtubeUrl">
    /// Updated YouTube URL or video identifier.
    /// </param>
    /// <param name="description">
    /// Updated description.
    /// </param>
    /// <param name="theme">
    /// Updated service theme.
    /// </param>
    /// <param name="isLive">
    /// Updated live status.
    /// </param>
    public void Update(
        string title,
        string youtubeUrl,
        string? description,
        string? theme,
        bool isLive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            title,
            nameof(title));

        Title = title.Trim();

        VideoId =
            ExtractVideoId(youtubeUrl);

        Description =
            NormalizeOptionalText(description);

        Theme =
            NormalizeOptionalText(theme);

        IsLive = isLive;

        MarkAsUpdated();
    }

    /// <summary>
    /// Publishes the broadcast so that it can be shown
    /// on the public website.
    /// </summary>
    public void Publish()
    {
        if (IsPublished)
        {
            return;
        }

        IsPublished = true;

        MarkAsUpdated();
    }

    /// <summary>
    /// Unpublishes the broadcast so that it is hidden
    /// from the public website.
    /// </summary>
    public void Unpublish()
    {
        if (!IsPublished)
        {
            return;
        }

        IsPublished = false;

        MarkAsUpdated();
    }

    /// <summary>
    /// Sets the publication status of the broadcast.
    /// </summary>
    /// <param name="isPublished">
    /// <see langword="true"/> to publish the broadcast;
    /// otherwise <see langword="false"/>.
    /// </param>
    public void SetPublished(
        bool isPublished)
    {
        if (IsPublished == isPublished)
        {
            return;
        }

        IsPublished = isPublished;

        MarkAsUpdated();
    }

    /// <summary>
    /// Changes the church service associated with the broadcast.
    /// </summary>
    /// <param name="churchServiceId">
    /// Identifier of the new church service.
    /// </param>
    /// <param name="category">
    /// Category of the new church service.
    /// </param>
    public void ChangeService(
        Guid churchServiceId,
        ServiceCategory category)
    {
        if (churchServiceId == Guid.Empty)
        {
            throw new ArgumentException(
                "A church service is required.",
                nameof(churchServiceId));
        }

        ChurchServiceId = churchServiceId;
        Category = category;

        MarkAsUpdated();
    }

    /// <summary>
    /// Normalizes optional text by trimming it and converting
    /// empty values to <see langword="null"/>.
    /// </summary>
    /// <param name="value">
    /// Value to normalize.
    /// </param>
    /// <returns>
    /// The normalized value.
    /// </returns>
    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    /// <summary>
    /// Extracts a YouTube video identifier from a supported
    /// YouTube URL or accepts a direct 11-character video identifier.
    /// </summary>
    /// <param name="input">
    /// YouTube URL or video identifier.
    /// </param>
    /// <returns>
    /// The extracted YouTube video identifier.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when a valid YouTube video identifier cannot be extracted.
    /// </exception>
    private static string ExtractVideoId(
        string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            input,
            nameof(input));

        input = input.Trim();

        if (Uri.TryCreate(
            input,
            UriKind.Absolute,
            out var uri))
        {
            var query =
                System.Web.HttpUtility
                    .ParseQueryString(
                        uri.Query);

            var videoId =
                query["v"];

            if (!string.IsNullOrWhiteSpace(
                videoId))
            {
                return videoId;
            }

            if (uri.Host.Contains(
                "youtu.be",
                StringComparison.OrdinalIgnoreCase))
            {
                var shortId =
                    uri.AbsolutePath
                        .Trim('/');

                if (!string.IsNullOrWhiteSpace(
                    shortId))
                {
                    return shortId;
                }
            }
        }

        if (input.Length == 11)
        {
            return input;
        }

        throw new ArgumentException(
            "Could not extract a YouTube video ID from the given input.",
            nameof(input));
    }
}