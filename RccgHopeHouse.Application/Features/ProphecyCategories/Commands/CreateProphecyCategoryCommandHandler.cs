using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Commands;

public class CreateProphecyCategoryCommandHandler
    : IRequestHandler<
        CreateProphecyCategoryCommand,
        ProphecyCategoryDto>
{
    private readonly IProphecyCategoryRepository _categoryRepository;
    private readonly IProphecyYearRepository _yearRepository;

    public CreateProphecyCategoryCommandHandler(
        IProphecyCategoryRepository categoryRepository,
        IProphecyYearRepository yearRepository)
    {
        _categoryRepository = categoryRepository;
        _yearRepository = yearRepository;
    }

    public async Task<ProphecyCategoryDto> Handle(
        CreateProphecyCategoryCommand request,
        CancellationToken ct)
    {
        _ = await _yearRepository.GetByIdAsync(
                request.ProphecyYearId,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyYear),
                request.ProphecyYearId);

        var exists =
            await _categoryRepository.ExistsByNameAsync(
                request.ProphecyYearId,
                request.Name,
                ct: ct);

        if (exists)
        {
            throw new InvalidOperationException(
                $"A prophecy category named '{request.Name.Trim()}' " +
                "already exists for this prophecy year.");
        }

        var category = ProphecyCategory.Create(
            request.ProphecyYearId,
            request.Name,
            request.DisplayOrder,
            request.Description);

        await _categoryRepository.AddAsync(
            category,
            ct);

        await _categoryRepository.SaveChangesAsync(ct);

        return ProphecyCategoryDto.FromEntity(
            category);
    }
}