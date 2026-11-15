using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Queries;

public class GetPublishedProphecyYearsQueryHandler
    : IRequestHandler<
        GetPublishedProphecyYearsQuery,
        IReadOnlyList<ProphecyYearDto>>
{
    private readonly IProphecyYearRepository _repository;

    public GetPublishedProphecyYearsQueryHandler(
        IProphecyYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProphecyYearDto>> Handle(
        GetPublishedProphecyYearsQuery request,
        CancellationToken ct)
    {
        var years =
            await _repository.GetPublishedAsync(ct);

        return years
            .Select(ProphecyYearDto.FromEntity)
            .ToList();
    }
}