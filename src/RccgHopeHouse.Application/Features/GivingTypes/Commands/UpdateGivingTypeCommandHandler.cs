using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GivingTypes.Commands;

/// <summary>
/// Handles updates to an existing giving type.
/// </summary>
public class UpdateGivingTypeCommandHandler
    : IRequestHandler<UpdateGivingTypeCommand, GivingTypeDto>
{
    private readonly IGivingTypeRepository _repository;

    public UpdateGivingTypeCommandHandler(
        IGivingTypeRepository repository) =>
        _repository = repository;

    public async Task<GivingTypeDto> Handle(
        UpdateGivingTypeCommand request,
        CancellationToken ct)
    {
        var givingType = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(GivingType), request.Id);

        givingType.Update(
            name: request.Name,
            description: request.Description,
            displayOrder: request.DisplayOrder);

        await _repository.UpdateAsync(givingType, ct);
        await _repository.SaveChangesAsync(ct);

        return GivingTypeDto.FromEntity(givingType);
    }
}