using MediatR;
using RccgHopeHouse.Application.Features.Bibles.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Bibles.Queries;

/// <summary>
/// Handles <see cref="GetAvailableBiblesQuery"/> by retrieving the available
/// translations from the external Bible service and mapping the Core models
/// to Application DTOs.
/// </summary>
public sealed class GetAvailableBiblesQueryHandler
    : IRequestHandler<
        GetAvailableBiblesQuery,
        IReadOnlyList<ApiBibleTranslationDto>>
{
    private readonly IApiBibleService _apiBibleService;

    public GetAvailableBiblesQueryHandler(
        IApiBibleService apiBibleService)
    {
        _apiBibleService = apiBibleService;
    }

    public async Task<IReadOnlyList<ApiBibleTranslationDto>> Handle(
        GetAvailableBiblesQuery request,
        CancellationToken cancellationToken)
    {
        var translations =
            await _apiBibleService.GetAvailableBiblesAsync(
                cancellationToken);

        return translations
            .Select(translation => new ApiBibleTranslationDto(
                Code: translation.Code,
                Name: translation.Name,
                Abbreviation: translation.Abbreviation))
            .ToList();
    }
}