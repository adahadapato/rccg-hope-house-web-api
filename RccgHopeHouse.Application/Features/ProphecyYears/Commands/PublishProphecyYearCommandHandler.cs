using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Commands;

public class PublishProphecyYearCommandHandler
    : IRequestHandler<PublishProphecyYearCommand, ProphecyYearDto>
{
    private readonly IProphecyYearRepository _repository;

    public PublishProphecyYearCommandHandler(
        IProphecyYearRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyYearDto> Handle(
        PublishProphecyYearCommand request,
        CancellationToken ct)
    {
        var prophecyYear =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyYear),
                request.Id);

        prophecyYear.Publish();

        await _repository.UpdateAsync(
            prophecyYear,
            ct);

        await _repository.SaveChangesAsync(ct);

        return ProphecyYearDto.FromEntity(
            prophecyYear);
    }
}