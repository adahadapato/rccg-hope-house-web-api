using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

/// <summary>
/// Command to upload a new gallery image.
/// Accepts binary data, generates thumbnails server-side (via Infrastructure), and persists metadata.
/// </summary>
public record UploadGalleryImageCommand(
    byte[] ImageData,
    string ContentType,
    Guid CategoryId,
    string Title,
    string AltText,
    string? Description,
    DateTime? EventDate,
    string? Photographer,
    List<string>? Tags) : IRequest<GalleryImageDto>;