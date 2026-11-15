using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Commands;

public class UpdateThemeOfTheYearCommandHandler
    : IRequestHandler<
        UpdateThemeOfTheYearCommand,
        ThemeOfTheYearDto>
{
    private readonly IThemeOfTheYearRepository _repository;

    public UpdateThemeOfTheYearCommandHandler(
        IThemeOfTheYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<ThemeOfTheYearDto> Handle(
        UpdateThemeOfTheYearCommand request,
        CancellationToken cancellationToken)
    {
        var theme =
            await _repository.GetByIdAsync(
                request.Id,
                cancellationToken)
            ?? throw new NotFoundException(
                nameof(ThemeOfTheYear),
                request.Id);

        theme.Update(
            themeTitle: request.ThemeTitle,
            scriptureText: request.ScriptureText,
            scriptureReference: request.ScriptureReference,
            primaryDescription: request.PrimaryDescription,
            secondaryDescription: request.SecondaryDescription,
            callToActionText: request.CallToActionText);

        await _repository.UpdateAsync(
            theme,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return theme.ToThemeOfTheYearDto();
    }
}