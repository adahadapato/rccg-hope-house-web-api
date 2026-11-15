using MediatR;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Commands;

public record UpdateThemeOfTheYearCommand(
    Guid Id,
    string ThemeTitle,
    string ScriptureText,
    string ScriptureReference,
    string PrimaryDescription,
    string? SecondaryDescription,
    string? CallToActionText)
    : IRequest<ThemeOfTheYearDto>;