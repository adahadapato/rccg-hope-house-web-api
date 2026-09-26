using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Prophecies.Queries;

public class GetProphecyByIdQueryHandler
    : IRequestHandler<GetProphecyByIdQuery, ProphecyDto>
{
    private readonly IProphecyRepository _repository;

    public GetProphecyByIdQueryHandler(
        IProphecyRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyDto> Handle(
        GetProphecyByIdQuery request,
        CancellationToken ct)
    {
        var prophecy =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(Prophecy),
                request.Id);

        return ProphecyDto.FromEntity(prophecy);
    }
}