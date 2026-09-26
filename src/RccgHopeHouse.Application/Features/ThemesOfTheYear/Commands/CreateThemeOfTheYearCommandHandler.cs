using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Commands;

public class CreateThemeOfTheYearCommandHandler
    : IRequestHandler<
        CreateThemeOfTheYearCommand,
        ThemeOfTheYearDto>
{
    private readonly IThemeOfTheYearRepository _repository;

    public CreateThemeOfTheYearCommandHandler(
        IThemeOfTheYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<ThemeOfTheYearDto> Handle(
        CreateThemeOfTheYearCommand request,
        CancellationToken cancellationToken)
    {
        var existing =
            await _repository.GetByYearAsync(
                request.Year,
                cancellationToken);

        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"A Theme of the Year already exists for {request.Year}.");
        }

        var theme =
            ThemeOfTheYear.Create(
                year: request.Year,
                themeTitle: request.ThemeTitle,
                scriptureText: request.ScriptureText,
                scriptureReference: request.ScriptureReference,
                primaryDescription: request.PrimaryDescription,
                secondaryDescription: request.SecondaryDescription,
                callToActionText: request.CallToActionText);

        await _repository.AddAsync(
            theme,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return theme.ToThemeOfTheYearDto();
    }
}