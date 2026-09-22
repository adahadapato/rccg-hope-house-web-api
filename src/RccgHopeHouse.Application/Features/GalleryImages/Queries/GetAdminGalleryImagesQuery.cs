using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

/// <summary>
/// Retrieves gallery images for administration.
///
/// Unlike the public gallery feed, this query includes both
/// public and private images and returns the full gallery DTO.
/// </summary>
public record GetAdminGalleryImagesQuery(
    Guid? CategoryId = null,
    Guid? TagId = null,
    bool? IsFeatured = null,
    int Skip = 0,
    int Take = 100)
    : IRequest<IReadOnlyList<GalleryImageDto>>;