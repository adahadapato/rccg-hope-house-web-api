using MediatR;

namespace RccgHopeHouse.Application.Features.Thanksgiving.Commands;

/// <summary>
/// Triggers background sync of thanksgiving videos from YouTube playlist.
/// Returns count of newly synced videos.
/// </summary>
public record SyncThanksgivingVideosCommand(string PlaylistId) : IRequest<int>;