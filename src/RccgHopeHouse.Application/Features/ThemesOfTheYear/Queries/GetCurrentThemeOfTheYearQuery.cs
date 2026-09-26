using MediatR;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Queries;

public record GetCurrentThemeOfTheYearQuery
    : IRequest<ThemeOfTheYearDto?>;
