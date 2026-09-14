using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Command to publish a draft post, making it visible in the public scrolling feed.
/// </summary>
public record PublishPastorPostCommand(Guid Id) : IRequest<PastorPostDto>;
