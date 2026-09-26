using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Prophecies.Queries;

public class GetLatestPropheciesQueryHandler
    : IRequestHandler<
        GetLatestPropheciesQuery,
        ProphecyYearDetailDto>
{
    private readonly IProphecyRepository _repository;

    public GetLatestPropheciesQueryHandler(
        IProphecyRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyYearDetailDto> Handle(
        GetLatestPropheciesQuery request,
        CancellationToken ct)
    {
        var prophecyYear =
            await _repository
                .GetLatestCompletePublishedYearAsync(ct)
            ?? throw new NotFoundException(
                nameof(ProphecyYear),
                "Latest");

        return ProphecyYearDetailDto.FromEntity(
            prophecyYear);
    }
}