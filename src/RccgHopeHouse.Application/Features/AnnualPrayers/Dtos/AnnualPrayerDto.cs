namespace RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;

/// <summary>
/// Represents annual prayer content returned
/// by the application/API layer.
/// </summary>
public record AnnualPrayerDto(
    Guid Id,
    int Year,
    string Theme,
    string Service,
    string Author,
    string BibleReference,
    string BibleText,
    string Declaration,
    string ClosingVerse,
    string? ImageUrl,
    bool IsActive,
    IReadOnlyList<AnnualPrayerPointDto> PrayerPoints);