using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Queries;

public class GetProphecyYearByIdQueryHandler
    : IRequestHandler<
        GetProphecyYearByIdQuery,
        ProphecyYearDto>
{
    private readonly IProphecyYearRepository _repository;

    public GetProphecyYearByIdQueryHandler(
        IProphecyYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyYearDto> Handle(
        GetProphecyYearByIdQuery request,
        CancellationToken ct)
    {
        var prophecyYear =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyYear),
                request.Id);

        return ProphecyYearDto.FromEntity(
            prophecyYear);
    }
}