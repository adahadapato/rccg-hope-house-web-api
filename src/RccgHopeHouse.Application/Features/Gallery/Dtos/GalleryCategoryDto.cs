namespace RccgHopeHouse.Application.Features.Gallery.Dtos;

public record GalleryCategoryDto(Guid Id, string Name, string? Description, int DisplayOrder, int ImageCount);