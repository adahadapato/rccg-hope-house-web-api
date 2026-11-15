using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Configuration;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IYouTubeService"/> using the official
/// Google YouTube Data API v3 client.
/// </summary>
public sealed class YouTubeService : IYouTubeService
{
    private const int YouTubeMaximumResults = 50;

    private readonly Google.Apis.YouTube.v3.YouTubeService _client;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="YouTubeService"/> class.
    /// </summary>
    /// <param name="configuration">
    /// The application configuration containing the YouTube
    /// API key and approved channel settings.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the YouTube API key has not been configured.
    /// </exception>
    public YouTubeService(IConfiguration configuration)
    {
        _configuration = configuration;

        var apiKey = configuration[AppSettings.YouTube.ApiKey];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("The YouTube API key has not been configured.");
        }

        _client = new Google.Apis.YouTube.v3.YouTubeService(
            new BaseClientService.Initializer
            {
                ApiKey = apiKey,
                ApplicationName = "RCCG-HopeHouse-Website"
            });
    }

    /// <inheritdoc />
    public IReadOnlyList<YouTubeChannelInfo> GetChannels()
    {
        var section =
            _configuration.GetSection(
                AppSettings.YouTube.Channels);

        var channels =
            new List<YouTubeChannelInfo>();

        foreach (var child in section.GetChildren())
        {
            var name = child["Name"];
            var channelId = child["ChannelId"];

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(channelId))
            {
                continue;
            }

            channels.Add(
                new YouTubeChannelInfo(
                    name.Trim(),
                    channelId.Trim()));
        }

        return channels;
    }

    /// <inheritdoc />
    public async Task<List<YouTubeVideoInfo>> GetPlaylistVideosAsync(
        string playlistId,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(playlistId))
        {
            throw new ArgumentException(
                "A YouTube playlist identifier is required.",
                nameof(playlistId));
        }

        var videoIds = new List<string>();
        string? nextPageToken = null;

        do
        {
            var request =
                _client.PlaylistItems.List(
                    "contentDetails");

            request.PlaylistId = playlistId.Trim();
            request.MaxResults = YouTubeMaximumResults;
            request.PageToken = nextPageToken;

            var response =
                await request.ExecuteAsync(ct);

            foreach (var item in response.Items)
            {
                var videoId =
                    item.ContentDetails?.VideoId;

                if (!string.IsNullOrWhiteSpace(videoId))
                {
                    videoIds.Add(videoId);
                }
            }

            nextPageToken = response.NextPageToken;
        }
        while (!string.IsNullOrWhiteSpace(nextPageToken));

        if (videoIds.Count == 0)
        {
            return [];
        }

        var videos = new List<YouTubeVideoInfo>();

        foreach (var batch in videoIds.Chunk(YouTubeMaximumResults))
        {
            var batchVideos =
                await GetVideosByIdsAsync(
                    batch,
                    ct);

            videos.AddRange(batchVideos);
        }

        return videos
            .OrderByDescending(video =>
                video.PublishedAt)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<YouTubeVideoInfo?> GetVideoDetailsAsync(
        string videoId,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(videoId))
        {
            throw new ArgumentException(
                "A YouTube video identifier is required.",
                nameof(videoId));
        }

        var videos =
            await GetVideosByIdsAsync(
                [videoId.Trim()],
                ct);

        return videos.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<List<YouTubeVideoInfo>> GetChannelVideosAsync(
        string channelId,
        int maxResults = 50,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(channelId))
        {
            throw new ArgumentException(
                "A YouTube channel identifier is required.",
                nameof(channelId));
        }

        var requestedResults = NormalizeMaxResults(maxResults);

        // Channel uploads playlists are much cheaper than Search.List.
        // Channels.List and PlaylistItems.List each normally cost 1 quota unit.
        var channelRequest = _client.Channels.List("contentDetails");
        channelRequest.Id = channelId.Trim();

        var channelResponse = await channelRequest.ExecuteAsync(ct);
        var uploadsPlaylistId = channelResponse.Items?
            .FirstOrDefault()?
            .ContentDetails?
            .RelatedPlaylists?
            .Uploads;

        if (string.IsNullOrWhiteSpace(uploadsPlaylistId))
        {
            return [];
        }

        // Fetch one page only; do not scan the entire channel history.
        var playlistRequest = _client.PlaylistItems.List("contentDetails");
        playlistRequest.PlaylistId = uploadsPlaylistId;
        playlistRequest.MaxResults = requestedResults;

        var playlistResponse = await playlistRequest.ExecuteAsync(ct);
        var videoIds = (playlistResponse.Items ?? [])
            .Select(item => item.ContentDetails?.VideoId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id!)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        return await GetVideosByIdsAsync(videoIds, ct);
    }

    /// <inheritdoc />
    public async Task<List<YouTubeVideoInfo>>
        GetUpcomingLiveStreamsAsync(
            string channelId,
            int maxResults = 25,
            CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(channelId))
        {
            throw new ArgumentException(
                "A YouTube channel identifier is required.",
                nameof(channelId));
        }

        var requestedResults =
            NormalizeMaxResults(maxResults);

        var request =
            _client.Search.List(
                "snippet");

        request.ChannelId = channelId.Trim();
        request.Type = "video";
        request.EventType =
            SearchResource.ListRequest.EventTypeEnum.Upcoming;
        request.Order =
            SearchResource.ListRequest.OrderEnum.Date;
        request.MaxResults = requestedResults;

        var response =
            await request.ExecuteAsync(ct);

        var videoIds = response.Items
            .Select(item => item.Id?.VideoId)
            .Where(videoId =>
                !string.IsNullOrWhiteSpace(videoId))
            .Select(videoId => videoId!)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        return await GetVideosByIdsAsync(
            videoIds,
            ct);
    }

    /// <summary>
    /// Retrieves full video and livestream information for
    /// the supplied YouTube video identifiers.
    /// </summary>
    /// <param name="videoIds">
    /// The YouTube video identifiers.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The videos returned by YouTube.
    /// </returns>
    private async Task<List<YouTubeVideoInfo>> GetVideosByIdsAsync(
        IEnumerable<string> videoIds,
        CancellationToken ct)
    {
        var ids = videoIds
            .Where(id =>
                !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .Take(YouTubeMaximumResults)
            .ToList();

        if (ids.Count == 0)
        {
            return [];
        }

        var request =
            _client.Videos.List(
                "snippet,liveStreamingDetails");

        request.Id =
            string.Join(
                ",",
                ids);

        var response =
            await request.ExecuteAsync(ct);

        return response.Items
            .Select(MapVideo)
            .OrderByDescending(video =>
                video.PublishedAt)
            .ToList();
    }

    /// <summary>
    /// Maps a YouTube video resource to the application's
    /// YouTube video information model.
    /// </summary>
    /// <param name="video">
    /// The YouTube video resource.
    /// </param>
    /// <returns>
    /// The mapped YouTube video information.
    /// </returns>
    private static YouTubeVideoInfo MapVideo(
        Video video)
    {
        var liveDetails =
            video.LiveStreamingDetails;

        var scheduledStart =
            liveDetails?
                .ScheduledStartTimeDateTimeOffset?
                .UtcDateTime;

        var actualStart =
            liveDetails?
                .ActualStartTimeDateTimeOffset?
                .UtcDateTime;

        var actualEnd =
            liveDetails?
                .ActualEndTimeDateTimeOffset?
                .UtcDateTime;

        return new YouTubeVideoInfo(
            video.Id,
            video.Snippet?.Title ?? string.Empty,
            video.Snippet?.Description ?? string.Empty,
            GetThumbnailUrl(video.Snippet?.Thumbnails),
            BuildVideoUrl(video.Id),
            GetPublishedAt(video.Snippet),
            scheduledStart,
            actualStart,
            actualEnd,
            DetermineBroadcastStatus(
                liveDetails,
                scheduledStart,
                actualStart,
                actualEnd));
    }

    /// <summary>
    /// Determines the broadcast status of a YouTube video
    /// from its livestream metadata.
    /// </summary>
    /// <param name="liveDetails">
    /// The YouTube livestream details.
    /// </param>
    /// <param name="scheduledStart">
    /// The scheduled livestream start time.
    /// </param>
    /// <param name="actualStart">
    /// The actual livestream start time.
    /// </param>
    /// <param name="actualEnd">
    /// The actual livestream end time.
    /// </param>
    /// <returns>
    /// The determined YouTube broadcast status.
    /// </returns>
    private static YouTubeBroadcastStatus DetermineBroadcastStatus(
        VideoLiveStreamingDetails? liveDetails,
        DateTime? scheduledStart,
        DateTime? actualStart,
        DateTime? actualEnd)
    {
        if (liveDetails is null)
        {
            return YouTubeBroadcastStatus.None;
        }

        if (actualEnd.HasValue)
        {
            return YouTubeBroadcastStatus.Completed;
        }

        if (actualStart.HasValue)
        {
            return YouTubeBroadcastStatus.Live;
        }

        if (scheduledStart.HasValue)
        {
            return YouTubeBroadcastStatus.Upcoming;
        }

        return YouTubeBroadcastStatus.None;
    }

    /// <summary>
    /// Builds the standard YouTube watch URL for a video.
    /// </summary>
    /// <param name="videoId">
    /// The YouTube video identifier.
    /// </param>
    /// <returns>
    /// The full YouTube watch URL.
    /// </returns>
    private static string BuildVideoUrl(
        string videoId)
    {
        return $"https://www.youtube.com/watch?v={videoId}";
    }

    /// <summary>
    /// Gets the best available thumbnail URL.
    /// </summary>
    /// <param name="thumbnails">
    /// The YouTube thumbnail collection.
    /// </param>
    /// <returns>
    /// The best available thumbnail URL.
    /// </returns>
    private static string GetThumbnailUrl(
        ThumbnailDetails? thumbnails)
    {
        return thumbnails?.Maxres?.Url ??
               thumbnails?.High?.Url ??
               thumbnails?.Standard?.Url ??
               thumbnails?.Medium?.Url ??
               thumbnails?.Default__?.Url ??
               string.Empty;
    }

    /// <summary>
    /// Gets the publication date from a video snippet.
    /// </summary>
    /// <param name="snippet">
    /// The YouTube video snippet.
    /// </param>
    /// <returns>
    /// The publication date when available; otherwise,
    /// the current UTC date and time.
    /// </returns>
    private static DateTime GetPublishedAt(
        VideoSnippet? snippet)
    {
        return snippet?
                   .PublishedAtDateTimeOffset?
                   .UtcDateTime ??
               DateTime.UtcNow;
    }

    /// <summary>
    /// Ensures the requested result count is within
    /// YouTube's permitted range.
    /// </summary>
    /// <param name="maxResults">
    /// The requested maximum number of results.
    /// </param>
    /// <returns>
    /// A valid maximum result count.
    /// </returns>
    private static int NormalizeMaxResults(
        int maxResults)
    {
        if (maxResults <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxResults),
                "The maximum number of results must be greater than zero.");
        }

        return Math.Min(
            maxResults,
            YouTubeMaximumResults);
    }
}