using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Commands;

public class DeactivateProphecyCategoryCommandHandler
    : IRequestHandler<
        DeactivateProphecyCategoryCommand,
        ProphecyCategoryDto>
{
    private readonly IProphecyCategoryRepository _repository;

    public DeactivateProphecyCategoryCommandHandler(
        IProphecyCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyCategoryDto> Handle(
        DeactivateProphecyCategoryCommand request,
        CancellationToken ct)
    {
        var category =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyCategory),
                request.Id);

        category.Deactivate();

        await _repository.UpdateAsync(
            category,
            ct);

        await _repository.SaveChangesAsync(ct);

        return ProphecyCategoryDto.FromEntity(
            category);
    }
}