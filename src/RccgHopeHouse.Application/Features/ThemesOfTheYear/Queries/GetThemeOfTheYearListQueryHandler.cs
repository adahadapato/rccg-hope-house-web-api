using MediatR;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Application.Common.Mappings;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Queries;

/// <summary>
/// Handles retrieval of all Theme of the Year records.
/// </summary>
public class GetThemeOfTheYearListQueryHandler
    : IRequestHandler<
        GetThemeOfTheYearListQuery,
        IReadOnlyList<ThemeOfTheYearDto>>
{
    private readonly IThemeOfTheYearRepository _repository;

    public GetThemeOfTheYearListQueryHandler(
        IThemeOfTheYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ThemeOfTheYearDto>> Handle(
        GetThemeOfTheYearListQuery request,
        CancellationToken cancellationToken)
    {
        var themes = await _repository.GetAllAsync(
            cancellationToken);

        return themes
            .Select(theme => theme.ToThemeOfTheYearDto())
            .ToList();
    }
}
