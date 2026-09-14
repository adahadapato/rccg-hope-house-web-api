using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

/// <summary>
/// Command to update gallery image metadata or replace binary data.
/// </summary>
public record UpdateGalleryImageCommand(
    Guid Id,
    string Title,
    string? Description,
    string AltText,
    string? Photographer,
    byte[]? NewImageData,
    string? NewContentType,
    DateTime? EventDate) : IRequest<GalleryImageDto>;