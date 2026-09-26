using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Command to unpublish a published Pastor's Corner post,
/// removing it from public view without deleting it.
/// </summary>
public record UnpublishPastorPostCommand(Guid Id)
    : IRequest<PastorPostDto>;