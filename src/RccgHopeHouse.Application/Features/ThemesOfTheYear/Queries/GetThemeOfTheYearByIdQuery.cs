using MediatR;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Queries;

public record GetThemeOfTheYearByIdQuery(
 Guid Id)
 : IRequest<ThemeOfTheYearDto>;
