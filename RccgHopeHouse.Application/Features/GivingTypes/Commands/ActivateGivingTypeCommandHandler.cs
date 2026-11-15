using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GivingTypes.Commands;

/// <summary>
/// Handles activation of an existing giving type.
/// </summary>
public class ActivateGivingTypeCommandHandler
    : IRequestHandler<ActivateGivingTypeCommand, GivingTypeDto>
{
    private readonly IGivingTypeRepository _repository;

    public ActivateGivingTypeCommandHandler(
        IGivingTypeRepository repository) =>
        _repository = repository;

    public async Task<GivingTypeDto> Handle(
        ActivateGivingTypeCommand request,
        CancellationToken ct)
    {
        var givingType = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(GivingType), request.Id);

        givingType.Activate();

        await _repository.UpdateAsync(givingType, ct);
        await _repository.SaveChangesAsync(ct);

        return GivingTypeDto.FromEntity(givingType);
    }
}