using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Thanksgiving.Commands;

/// <summary>
/// Handler for syncing YouTube playlist videos to database.
/// Uses domain factory method for entity creation.
/// </summary>
public class SyncThanksgivingVideosCommandHandler : IRequestHandler<SyncThanksgivingVideosCommand, int>
{
    private readonly IThanksgivingRepository _repository;
    private readonly IYouTubeService _youtubeService;

    public SyncThanksgivingVideosCommandHandler(
        IThanksgivingRepository repository,
        IYouTubeService youtubeService)
    {
        _repository = repository;
        _youtubeService = youtubeService;
    }

    /// <summary>
    /// Fetches videos from YouTube, filters out duplicates, and persists new entries using domain factory.
    /// </summary>
    public async Task<int> Handle(
        SyncThanksgivingVideosCommand request,
        CancellationToken cancellationToken)
    {
        var videos = await _youtubeService.GetPlaylistVideosAsync(request.PlaylistId, cancellationToken);
        int synced = 0;

        foreach (var v in videos)
        {
            // Check for existing video by URL (deduplication)
            var existing = await _repository.GetByVideoUrlAsync(v.VideoUrl, cancellationToken);
            if (existing == null)
            {
                // ✅ Use domain factory method, not constructor
                var service = ThanksgivingService.Create(
                    title: v.Title,
                    videoUrl: v.VideoUrl,
                    publishedAt: v.PublishedAt,
                    thumbnailUrl: v.ThumbnailUrl,
                    description: v.Description,
                    isAutoSynced: true);

                await _repository.AddAsync(service, cancellationToken);
                synced++;
            }
        }

        if (synced > 0)
            await _repository.SaveChangesAsync(cancellationToken);

        return synced;
    }
}