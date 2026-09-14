using MediatR;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Command to toggle the pinned status of a post.
/// Pinned posts appear at the top of the scrolling feed.
/// </summary>
public record PinPastorPostCommand(Guid Id, bool IsPinned) : IRequest<Unit>;
