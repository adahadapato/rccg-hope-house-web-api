namespace RccgHopeHouse.Application.Features.Sermons.Dtos
{
    public record SermonDto(Guid Id,
    string Title,
    string Speaker,
    DateTime ServiceDate,
    string VideoUrl,
    string? Description,
    bool IsPublished,
    DateTime CreatedAt);
    
    
}
