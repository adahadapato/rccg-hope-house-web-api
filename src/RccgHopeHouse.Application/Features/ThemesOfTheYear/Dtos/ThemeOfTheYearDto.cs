namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;

/// <summary>
/// Represents an annual church theme returned by the application/API layer.
/// </summary>
public record ThemeOfTheYearDto(
    Guid Id,
    int Year,
    string ThemeTitle,
    string ScriptureText,
    string ScriptureReference,
    string PrimaryDescription,
    string? SecondaryDescription,
    string? CallToActionText);