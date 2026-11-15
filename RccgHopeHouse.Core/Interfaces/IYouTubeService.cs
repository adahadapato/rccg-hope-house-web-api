namespace RccgHopeHouse.Core.Interfaces;


/// <summary>
/// Represents information about a YouTube video.
/// </summary>
/// <param name="VideoId">
/// The unique YouTube video identifier.
/// </param>
/// <param name="Title">
/// The title of the video.
/// </param>
/// <param name="Description">
/// The description of the video.
/// </param>
/// <param name="ThumbnailUrl">
/// The URL of the preferred video thumbnail.
/// </param>
/// <param name="VideoUrl">
/// The full YouTube watch URL for the video.
/// </param>
/// <param name="PublishedAt">
/// The date and time the video was published.
/// </param>
/// <param name="ScheduledStartTime">
/// The scheduled start time of a livestream, when available.
/// </param>
/// <param name="ActualStartTime">
/// The actual start time of a livestream, when available.
/// </param>
/// <param name="ActualEndTime">
/// The actual end time of a livestream, when available.
/// </param>
/// <param name="BroadcastStatus">
/// The current or historical broadcast status of the video.
/// </param>
public record YouTubeVideoInfo(
    string VideoId,
    string Title,
    string Description,
    string ThumbnailUrl,
    string VideoUrl,
    DateTime PublishedAt,
    DateTime? ScheduledStartTime,
    DateTime? ActualStartTime,
    DateTime? ActualEndTime,
    YouTubeBroadcastStatus BroadcastStatus);

/// <summary>
/// Represents a YouTube channel configured as an approved
/// source for service broadcast synchronization.
/// </summary>
/// <param name="Name">
/// The friendly name of the YouTube channel.
/// </param>
/// <param name="ChannelId">
/// The unique YouTube channel identifier.
/// </param>
public sealed record YouTubeChannelInfo(
    string Name,
    string ChannelId);


/// <summary>
/// Defines operations for retrieving public YouTube
/// video and channel content.
/// </summary>
public interface IYouTubeService
{

    /// <summary>
    /// Retrieves the YouTube channels configured for
    /// automatic service broadcast synchronization.
    /// </summary>
    /// <returns>
    /// The configured YouTube channels.
    /// </returns>
    public IReadOnlyList<YouTubeChannelInfo> GetChannels();


    /// <summary>
    /// Retrieves all videos contained in a YouTube playlist.
    /// </summary>
    /// <param name="playlistId">
    /// The YouTube playlist identifier.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The videos contained in the playlist.
    /// </returns>
    Task<List<YouTubeVideoInfo>> GetPlaylistVideosAsync(
        string playlistId,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieves details for a specific YouTube video.
    /// </summary>
    /// <param name="videoId">
    /// The YouTube video identifier.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The video information when found; otherwise, null.
    /// </returns>
    Task<YouTubeVideoInfo?> GetVideoDetailsAsync(
        string videoId,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieves the most recently published videos
    /// from a specific YouTube channel.
    /// </summary>
    /// <param name="channelId">
    /// The YouTube channel identifier.
    /// </param>
    /// <param name="maxResults">
    /// The maximum number of videos to retrieve.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The videos retrieved from the channel.
    /// </returns>
    Task<List<YouTubeVideoInfo>> GetChannelVideosAsync(string channelId, int maxResults = 50, CancellationToken ct = default);

    /// <summary>
    /// Retrieves upcoming scheduled livestreams
    /// from a specific YouTube channel.
    /// </summary>
    /// <param name="channelId">
    /// The YouTube channel identifier.
    /// </param>
    /// <param name="maxResults">
    /// The maximum number of videos to retrieve.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The upcoming livestreams found on the channel.
    /// </returns>
    Task<List<YouTubeVideoInfo>> GetUpcomingLiveStreamsAsync(
        string channelId,
        int maxResults = 25,
        CancellationToken ct = default);
}