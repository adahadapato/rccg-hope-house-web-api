using MediatR;


using RccgHopeHouse.Application.Features.Bibles.Dtos;


/// <summary>
/// Query for retrieving a Bible passage from a specified API.Bible resource.
/// </summary>
/// <param name="BibleId">
/// API.Bible identifier of the Bible resource.
/// </param>
/// <param name="PassageId">
/// API.Bible/USFM passage identifier, for example EXO.14.1-EXO.14.4.
/// </param>
public sealed record GetBiblePassageQuery(
    string BibleId,
    string PassageId)
    : IRequest<BiblePassageDto>;