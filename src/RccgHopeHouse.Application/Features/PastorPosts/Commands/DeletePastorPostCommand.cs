using MediatR;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Command to permanently delete a pastor post.
/// Use with caution: removes binary data and metadata from database.
/// </summary>
public record DeletePastorPostCommand(Guid Id) : IRequest<Unit>;
