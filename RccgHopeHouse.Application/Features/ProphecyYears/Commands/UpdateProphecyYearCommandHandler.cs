using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Commands;

public class UpdateProphecyYearCommandHandler
    : IRequestHandler<UpdateProphecyYearCommand, ProphecyYearDto>
{
    private readonly IProphecyYearRepository _repository;

    public UpdateProphecyYearCommandHandler(
        IProphecyYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyYearDto> Handle(
        UpdateProphecyYearCommand request,
        CancellationToken ct)
    {
        var prophecyYear =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyYear),
                request.Id);

        var exists = await _repository.ExistsAsync(
            request.Year,
            request.Id,
            ct);

        if (exists)
        {
            throw new InvalidOperationException(
                $"Prophecy year {request.Year} already exists.");
        }

        prophecyYear.ChangeYear(
            request.Year);

        await _repository.UpdateAsync(
            prophecyYear,
            ct);

        await _repository.SaveChangesAsync(ct);

        return ProphecyYearDto.FromEntity(
            prophecyYear);
    }
}