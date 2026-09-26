using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Commands;

public class UpdateProphecyCategoryCommandHandler
    : IRequestHandler<
        UpdateProphecyCategoryCommand,
        ProphecyCategoryDto>
{
    private readonly IProphecyCategoryRepository _repository;

    public UpdateProphecyCategoryCommandHandler(
        IProphecyCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyCategoryDto> Handle(
        UpdateProphecyCategoryCommand request,
        CancellationToken ct)
    {
        var category =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyCategory),
                request.Id);

        var exists =
            await _repository.ExistsByNameAsync(
                category.ProphecyYearId,
                request.Name,
                request.Id,
                ct);

        if (exists)
        {
            throw new InvalidOperationException(
                $"A prophecy category named '{request.Name.Trim()}' " +
                "already exists for this prophecy year.");
        }

        category.Update(
            request.Name,
            request.Description,
            request.DisplayOrder);

        await _repository.UpdateAsync(
            category,
            ct);

        await _repository.SaveChangesAsync(ct);

        return ProphecyCategoryDto.FromEntity(
            category);
    }
}