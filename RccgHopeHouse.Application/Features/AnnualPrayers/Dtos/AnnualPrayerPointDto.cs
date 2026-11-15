namespace RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;

/// <summary>
/// Represents an individual annual prayer point
/// returned by the application/API layer.
/// </summary>
public record AnnualPrayerPointDto(
    Guid Id,
    string Text,
    int DisplayOrder);