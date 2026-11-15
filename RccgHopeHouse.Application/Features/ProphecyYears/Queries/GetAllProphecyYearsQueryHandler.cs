using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Queries;

public class GetAllProphecyYearsQueryHandler
    : IRequestHandler<
        GetAllProphecyYearsQuery,
        IReadOnlyList<ProphecyYearDto>>
{
    private readonly IProphecyYearRepository _repository;

    public GetAllProphecyYearsQueryHandler(
        IProphecyYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProphecyYearDto>> Handle(
        GetAllProphecyYearsQuery request,
        CancellationToken ct)
    {
        var years = await _repository.GetAllAsync(ct);

        return years
            .Select(ProphecyYearDto.FromEntity)
            .ToList();
    }
}