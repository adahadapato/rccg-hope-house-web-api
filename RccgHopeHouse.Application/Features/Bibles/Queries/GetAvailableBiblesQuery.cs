using MediatR;


using RccgHopeHouse.Application.Features.Bibles.Dtos;


/// <summary>
/// Query for retrieving the Bible translations currently available
/// through the configured API.Bible integration.
/// </summary>
public sealed record GetAvailableBiblesQuery
    : IRequest<IReadOnlyList<ApiBibleTranslationDto>>;