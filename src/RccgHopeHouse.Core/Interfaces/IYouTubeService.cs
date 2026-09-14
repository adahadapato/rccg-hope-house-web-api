namespace RccgHopeHouse.Core.Interfaces
{
    public record YouTubeVideoInfo(
    string VideoId,
    string Title,
    string Description,
    string ThumbnailUrl,
    string VideoUrl,
    DateTime PublishedAt);
    
        
    

    public interface IYouTubeService
    {
        Task<List<YouTubeVideoInfo>> GetPlaylistVideosAsync(string playlistId, CancellationToken ct = default);
        Task<YouTubeVideoInfo?> GetVideoDetailsAsync(string videoId, CancellationToken ct = default);
    }
}
