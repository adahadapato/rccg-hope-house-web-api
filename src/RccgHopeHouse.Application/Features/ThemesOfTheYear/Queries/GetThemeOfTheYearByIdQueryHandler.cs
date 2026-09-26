using MediatR;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Application.Common.Mappings;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Queries;

public class GetThemeOfTheYearByIdQueryHandler
 : IRequestHandler<
     GetThemeOfTheYearByIdQuery,
     ThemeOfTheYearDto>
{
    private readonly IThemeOfTheYearRepository _repository;

    public GetThemeOfTheYearByIdQueryHandler(
        IThemeOfTheYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<ThemeOfTheYearDto> Handle(
        GetThemeOfTheYearByIdQuery request,
        CancellationToken cancellationToken)
    {
        var theme =
            await _repository.GetByIdAsync(
                request.Id,
                cancellationToken)
            ?? throw new NotFoundException(
                nameof(ThemeOfTheYear),
                request.Id);

        return theme.ToThemeOfTheYearDto();
    }
}
