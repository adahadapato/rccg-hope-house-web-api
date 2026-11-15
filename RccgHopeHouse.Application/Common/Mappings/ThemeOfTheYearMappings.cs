using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Common.Mappings;

public static class ThemeOfTheYearMappings
{
    public static ThemeOfTheYearDto ToThemeOfTheYearDto(
        this ThemeOfTheYear theme)
    {
        return new ThemeOfTheYearDto(
            Id: theme.Id,
            Year: theme.Year,
            ThemeTitle: theme.ThemeTitle,
            ScriptureText: theme.ScriptureText,
            ScriptureReference: theme.ScriptureReference,
            PrimaryDescription: theme.PrimaryDescription,
            SecondaryDescription: theme.SecondaryDescription,
            CallToActionText: theme.CallToActionText);
    }
}