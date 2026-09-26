using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Prophecies.Queries;

public class GetPropheciesByYearQueryHandler
    : IRequestHandler<
        GetPropheciesByYearQuery,
        ProphecyYearDetailDto>
{
    private readonly IProphecyRepository _repository;

    public GetPropheciesByYearQueryHandler(
        IProphecyRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyYearDetailDto> Handle(
        GetPropheciesByYearQuery request,
        CancellationToken ct)
    {
        var prophecyYear =
            await _repository.GetCompleteYearAsync(
                request.Year,
                publishedOnly: true,
                activeOnly: true,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyYear),
                request.Year);

        return ProphecyYearDetailDto.FromEntity(
            prophecyYear);
    }
}