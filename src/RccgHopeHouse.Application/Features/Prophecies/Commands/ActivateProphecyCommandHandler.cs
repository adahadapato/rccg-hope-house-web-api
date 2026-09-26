using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Prophecies.Commands;

public class ActivateProphecyCommandHandler
    : IRequestHandler<ActivateProphecyCommand, ProphecyDto>
{
    private readonly IProphecyRepository _prophecyRepository;
    private readonly IProphecyCategoryRepository _categoryRepository;

    public ActivateProphecyCommandHandler(
        IProphecyRepository prophecyRepository,
        IProphecyCategoryRepository categoryRepository)
    {
        _prophecyRepository = prophecyRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProphecyDto> Handle(
        ActivateProphecyCommand request,
        CancellationToken ct)
    {
        var prophecy =
            await _prophecyRepository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(Prophecy),
                request.Id);

        var category =
            await _categoryRepository.GetByIdAsync(
                prophecy.CategoryId,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyCategory),
                prophecy.CategoryId);

        if (!category.IsActive)
        {
            throw new InvalidOperationException(
                "A prophecy cannot be activated while its category is inactive.");
        }

        prophecy.Activate();

        await _prophecyRepository.UpdateAsync(
            prophecy,
            ct);

        await _prophecyRepository.SaveChangesAsync(ct);

        return ProphecyDto.FromEntity(prophecy);
    }
}