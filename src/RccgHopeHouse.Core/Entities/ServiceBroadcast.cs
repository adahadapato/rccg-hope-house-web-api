using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Entities;

public class ServiceBroadcast : BaseEntity
{
    /// <summary>
    /// Retained for compatibility and useful classification,
    /// but ChurchServiceId is the authoritative relationship
    /// between a broadcast and a particular service.
    /// </summary>
    public ServiceCategory Category { get; private set; }

    public Guid ChurchServiceId { get; private set; }

    public ChurchService ChurchService { get; private set; } = null!;

    public string Title { get; private set; } = string.Empty;

    public string VideoId { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTime ServiceMonth { get; private set; }

    public bool IsLive { get; private set; }

    /// <summary>
    /// This month's sermon/service theme badge.
    /// </summary>
    public string? Theme { get; private set; }

    public string ThumbnailUrl =>
        $"https://i.ytimg.com/vi/{VideoId}/hqdefault.jpg";

    public string VideoUrl =>
        $"https://www.youtube.com/watch?v={VideoId}";

    private ServiceBroadcast()
    {
    }

    public static ServiceBroadcast Create(
        Guid churchServiceId,
        ServiceCategory category,
        string title,
        string youtubeUrl,
        DateTime serviceMonth,
        string? description = null,
        string? theme = null,
        bool isLive = false)
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
            IsLive = isLive
        };
    }

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

    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static string ExtractVideoId(
        string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            input,
            nameof(input));

        input = input.Trim();

        if (
            Uri.TryCreate(
                input,
                UriKind.Absolute,
                out var uri)
        )
        {
            var query =
                System.Web.HttpUtility
                    .ParseQueryString(
                        uri.Query);

            var videoId =
                query["v"];

            if (
                !string.IsNullOrWhiteSpace(
                    videoId)
            )
            {
                return videoId;
            }

            if (
                uri.Host.Contains(
                    "youtu.be",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                var shortId =
                    uri.AbsolutePath
                        .Trim('/');

                if (
                    !string.IsNullOrWhiteSpace(
                        shortId)
                )
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