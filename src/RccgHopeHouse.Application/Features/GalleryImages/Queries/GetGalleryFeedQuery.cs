using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

/// <summary>
/// Query to retrieve the public gallery feed.
/// Supports pagination, category filtering, tag filtering, and featured highlighting.
/// Optimized: returns lightweight DTOs, excludes full binary image data.
/// </summary>
public record GetGalleryFeedQuery(
    Guid? CategoryId = null,
    Guid? TagId = null,
    bool? IsFeatured = null,
    int Skip = 0,
    int Take = 20) : IRequest<IReadOnlyList<GalleryImageFeedDto>>;