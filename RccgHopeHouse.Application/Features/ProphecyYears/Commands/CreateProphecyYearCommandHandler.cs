using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Commands;

public class CreateProphecyYearCommandHandler
    : IRequestHandler<CreateProphecyYearCommand, ProphecyYearDto>
{
    private readonly IProphecyYearRepository _repository;

    public CreateProphecyYearCommandHandler(
        IProphecyYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyYearDto> Handle(
        CreateProphecyYearCommand request,
        CancellationToken ct)
    {
        var exists = await _repository.ExistsAsync(
            request.Year,
            ct: ct);

        if (exists)
        {
            throw new InvalidOperationException(
                $"Prophecy year {request.Year} already exists.");
        }

        var prophecyYear = ProphecyYear.Create(
            request.Year);

        await _repository.AddAsync(
            prophecyYear,
            ct);

        await _repository.SaveChangesAsync(ct);

        return ProphecyYearDto.FromEntity(
            prophecyYear);
    }
}