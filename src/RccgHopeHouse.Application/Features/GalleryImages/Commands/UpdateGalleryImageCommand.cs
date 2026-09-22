using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

/// <summary>
/// Command to update gallery image metadata and optionally
/// replace the stored physical image.
///
/// Category, tags and display order can also be changed.
/// </summary>
public record UpdateGalleryImageCommand(
    Guid Id,
    string Title,
    string? Description,
    string AltText,
    string? Photographer,
    byte[]? NewImageData,
    string? NewContentType,
    DateTime? EventDate,
    Guid CategoryId,
    List<string>? Tags,
    int DisplayOrder)
    : IRequest<GalleryImageDto>;