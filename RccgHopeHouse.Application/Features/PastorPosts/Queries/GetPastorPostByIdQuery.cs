using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;

namespace RccgHopeHouse.Application.Features.PastorPosts.Queries;

/// <summary>
/// Query to retrieve a single published post by ID.
/// Loads full content and cover image for detailed view.
/// </summary>
public record GetPastorPostByIdQuery(Guid Id) : IRequest<PastorPostDto>;
