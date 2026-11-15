using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Prophecies.Commands;

public class CreateProphecyCommandHandler
    : IRequestHandler<CreateProphecyCommand, ProphecyDto>
{
    private readonly IProphecyRepository _prophecyRepository;
    private readonly IProphecyCategoryRepository _categoryRepository;

    public CreateProphecyCommandHandler(
        IProphecyRepository prophecyRepository,
        IProphecyCategoryRepository categoryRepository)
    {
        _prophecyRepository = prophecyRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProphecyDto> Handle(
        CreateProphecyCommand request,
        CancellationToken ct)
    {
        var category =
            await _categoryRepository.GetByIdAsync(
                request.CategoryId,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyCategory),
                request.CategoryId);

        if (!category.IsActive)
        {
            throw new InvalidOperationException(
                "A prophecy cannot be added to an inactive prophecy category.");
        }

        var prophecy = Prophecy.Create(
            request.CategoryId,
            request.Text,
            request.DisplayOrder);

        await _prophecyRepository.AddAsync(
            prophecy,
            ct);

        await _prophecyRepository.SaveChangesAsync(ct);

        return ProphecyDto.FromEntity(prophecy);
    }
}