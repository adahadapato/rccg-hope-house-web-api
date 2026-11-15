using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Prophecies.Commands;

public class DeactivateProphecyCommandHandler
    : IRequestHandler<DeactivateProphecyCommand, ProphecyDto>
{
    private readonly IProphecyRepository _repository;

    public DeactivateProphecyCommandHandler(
        IProphecyRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyDto> Handle(
        DeactivateProphecyCommand request,
        CancellationToken ct)
    {
        var prophecy =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(Prophecy),
                request.Id);

        prophecy.Deactivate();

        await _repository.UpdateAsync(
            prophecy,
            ct);

        await _repository.SaveChangesAsync(ct);

        return ProphecyDto.FromEntity(prophecy);
    }
}