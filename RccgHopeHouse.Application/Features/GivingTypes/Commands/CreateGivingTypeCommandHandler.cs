using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GivingTypes.Commands;

/// <summary>
/// Handles the creation of a new giving type.
/// </summary>
public class CreateGivingTypeCommandHandler
    : IRequestHandler<CreateGivingTypeCommand, GivingTypeDto>
{
    private readonly IGivingTypeRepository _repository;

    public CreateGivingTypeCommandHandler(
        IGivingTypeRepository repository) =>
        _repository = repository;

    public async Task<GivingTypeDto> Handle(
        CreateGivingTypeCommand request,
        CancellationToken ct)
    {
        var givingType = GivingType.Create(
            name: request.Name,
            displayOrder: request.DisplayOrder,
            description: request.Description);

        await _repository.AddAsync(givingType, ct);
        await _repository.SaveChangesAsync(ct);

        return GivingTypeDto.FromEntity(givingType);
    }
}