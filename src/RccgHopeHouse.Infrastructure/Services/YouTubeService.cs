using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IYouTubeService"/> using the official Google YouTube Data API v3 client.
/// Fetches playlist metadata for automatic Thanksgiving service synchronization.
/// </summary>
public class YouTubeService : IYouTubeService
{
    private readonly Google.Apis.YouTube.v3.YouTubeService _client;
    private readonly string _apiKey;

    /// <summary>
    /// Initializes the YouTube client with API key and base configuration.
    /// </summary>
    public YouTubeService(IConfiguration configuration)
    {
        _apiKey = configuration["YouTube:ApiKey"] ?? throw new InvalidOperationException("YouTube API Key not configured.");
        _client = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = _apiKey,
            ApplicationName = "RCCG-HopeHouse-Website"
        });
    }

    /// <inheritdoc />
    /// <remarks>
    /// Paginates through playlist items until all videos are retrieved.
    /// Respects YouTube API quota limits (10,000 units/day).
    /// </remarks>
    public async Task<List<YouTubeVideoInfo>> GetPlaylistVideosAsync(string playlistId, CancellationToken ct = default)
    {
        var videos = new List<YouTubeVideoInfo>();
        string? nextPageToken = null;

        do
        {
            var request = _client.PlaylistItems.List("snippet,contentDetails");
            request.PlaylistId = playlistId;
            request.MaxResults = 50;
            request.PageToken = nextPageToken;

            var response = await request.ExecuteAsync(ct);

            foreach (var item in response.Items)
            {
                videos.Add(new YouTubeVideoInfo(
                    item.Snippet.ResourceId.VideoId,
                    item.Snippet.Title,
                    item.Snippet.Description,
                    item.Snippet.Thumbnails.High?.Url ?? item.Snippet.Thumbnails.Standard?.Url ?? string.Empty,
                    item.Snippet.ChannelId ?? $"https://www.youtube.com/watch?v={item.Id}",
                    item.Snippet.PublishedAtDateTimeOffset?.DateTime ?? DateTime.UtcNow
                ));
            }

            nextPageToken = response.NextPageToken;
        } while (nextPageToken != null);

        return videos;
    }

    /// <inheritdoc />
    public async Task<YouTubeVideoInfo?> GetVideoDetailsAsync(string videoId, CancellationToken ct = default)
    {
        var request = _client.Videos.List("snippet");
        request.Id = videoId;
        var response = await request.ExecuteAsync(ct);
        var item = response.Items.FirstOrDefault();

        return item == null ? null : new YouTubeVideoInfo(
            item.Id,
            item.Snippet.Title,
            item.Snippet.Description,
            item.Snippet.Thumbnails.High?.Url ?? string.Empty,
            item.Snippet.ChannelId ?? $"https://www.youtube.com/watch?v={item.Id}",
            item.Snippet.PublishedAtDateTimeOffset?.DateTime ?? DateTime.UtcNow
        );
    }
}