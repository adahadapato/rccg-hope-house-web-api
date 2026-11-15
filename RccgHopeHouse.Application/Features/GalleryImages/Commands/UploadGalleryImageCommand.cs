using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

/// <summary>
/// Command to upload a new gallery image.
///
/// The uploaded binary data is processed by Infrastructure,
/// while SQL Server stores the resulting paths and metadata.
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
    List<string>? Tags,
    int DisplayOrder = 0)
    : IRequest<GalleryImageDto>;