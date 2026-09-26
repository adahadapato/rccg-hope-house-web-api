using MediatR;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Queries;

/// <summary>
/// Gets all Theme of the Year records, ordered by year descending.
/// Used by administration screens and anywhere a theme selection list is required.
/// </summary>
public record GetThemeOfTheYearListQuery
    : IRequest<IReadOnlyList<ThemeOfTheYearDto>>;
