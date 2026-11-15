using MediatR;
using RccgHopeHouse.Application.Features.Bibles.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Bibles.Queries;

/// <summary>
/// Handles Bible passage retrieval and maps the Core model to the
/// Application response DTO.
/// </summary>
public sealed class GetBiblePassageQueryHandler
    : IRequestHandler<GetBiblePassageQuery, BiblePassageDto>
{
    private readonly IApiBibleService _apiBibleService;

    public GetBiblePassageQueryHandler(
        IApiBibleService apiBibleService)
    {
        _apiBibleService = apiBibleService;
    }

    public async Task<BiblePassageDto> Handle(
        GetBiblePassageQuery request,
        CancellationToken cancellationToken)
    {
        var passage = await _apiBibleService.GetPassageAsync(
            request.BibleId,
            request.PassageId,
            cancellationToken);

        return new BiblePassageDto(
            BibleId: passage.BibleId,
            Reference: passage.Reference,
            Content: passage.Content,
            Copyright: passage.Copyright);
    }
}
