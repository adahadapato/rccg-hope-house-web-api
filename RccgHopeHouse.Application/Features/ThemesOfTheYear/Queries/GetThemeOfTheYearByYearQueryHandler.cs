using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Queries;

public class GetThemeOfTheYearByYearQueryHandler
 : IRequestHandler<
     GetThemeOfTheYearByYearQuery,
     ThemeOfTheYearDto?>
{
    private readonly IThemeOfTheYearRepository _repository;

    public GetThemeOfTheYearByYearQueryHandler(
        IThemeOfTheYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<ThemeOfTheYearDto?> Handle(
        GetThemeOfTheYearByYearQuery request,
        CancellationToken cancellationToken)
    {
        var theme =
            await _repository.GetByYearAsync(
                request.Year,
                cancellationToken);

        return theme?.ToThemeOfTheYearDto();
    }
}
