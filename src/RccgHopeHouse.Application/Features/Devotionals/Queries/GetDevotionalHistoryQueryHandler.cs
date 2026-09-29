using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Queries;

public class GetDevotionalHistoryQueryHandler
    : IRequestHandler<GetDevotionalHistoryQuery, IReadOnlyList<DevotionalHistoryDto>>
{
    private readonly IDevotionalRepository _repository;

    public GetDevotionalHistoryQueryHandler(IDevotionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<DevotionalHistoryDto>> Handle(
        GetDevotionalHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var devotionals = await _repository.GetPublishedHistoryAsync(
            upToDate: request.Today,
            skip: request.Skip,
            take: request.Take,
            ct: cancellationToken);

        return devotionals
            .Select(DevotionalHistoryDto.FromEntity)
            .ToList();
    }
}