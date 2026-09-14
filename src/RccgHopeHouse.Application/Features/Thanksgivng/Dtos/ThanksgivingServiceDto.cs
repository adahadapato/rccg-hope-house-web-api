namespace RccgHopeHouse.Application.Features.Thanksgiving.Dtos;

/// <summary>
/// Lightweight DTO for public thanksgiving service feed.
/// Excludes heavy binary data; includes YouTube embed info.
/// </summary>
public record ThanksgivingServiceDto(
    Guid Id,
    string Title,
    string VideoUrl,
    string ThumbnailUrl,
    DateTime ServiceMonth,
    bool IsAutoSynced);