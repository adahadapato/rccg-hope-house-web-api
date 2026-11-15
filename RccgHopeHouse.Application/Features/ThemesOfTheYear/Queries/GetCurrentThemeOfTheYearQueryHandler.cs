using MediatR;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Application.Common.Mappings;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Queries;

public class GetCurrentThemeOfTheYearQueryHandler
: IRequestHandler<
    GetCurrentThemeOfTheYearQuery,
    ThemeOfTheYearDto?>
{
    private readonly IThemeOfTheYearRepository _repository;

    public GetCurrentThemeOfTheYearQueryHandler(
        IThemeOfTheYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<ThemeOfTheYearDto?> Handle(
        GetCurrentThemeOfTheYearQuery request,
        CancellationToken cancellationToken)
    {
        var theme =
            await _repository.GetCurrentYearThemeAsync(
                cancellationToken);

        theme ??=
            await _repository.GetLatestAsync(
                cancellationToken);

        return theme?.ToThemeOfTheYearDto();
    }
}
