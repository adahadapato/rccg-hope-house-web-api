using MediatR;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Queries;

public record GetThemeOfTheYearByYearQuery(
    int Year)
    : IRequest<ThemeOfTheYearDto?>;
