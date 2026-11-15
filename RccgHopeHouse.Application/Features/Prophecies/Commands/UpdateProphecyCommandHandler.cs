using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Prophecies.Commands;

public class UpdateProphecyCommandHandler
    : IRequestHandler<UpdateProphecyCommand, ProphecyDto>
{
    private readonly IProphecyRepository _prophecyRepository;
    private readonly IProphecyCategoryRepository _categoryRepository;

    public UpdateProphecyCommandHandler(
        IProphecyRepository prophecyRepository,
        IProphecyCategoryRepository categoryRepository)
    {
        _prophecyRepository = prophecyRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProphecyDto> Handle(
        UpdateProphecyCommand request,
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
                request.CategoryId,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyCategory),
                request.CategoryId);

        if (prophecy.CategoryId != request.CategoryId &&
            !category.IsActive)
        {
            throw new InvalidOperationException(
                "A prophecy cannot be moved to an inactive prophecy category.");
        }

        prophecy.Update(
            request.Text,
            request.DisplayOrder);

        if (prophecy.CategoryId != request.CategoryId)
        {
            prophecy.ChangeCategory(
                request.CategoryId);
        }

        await _prophecyRepository.UpdateAsync(
            prophecy,
            ct);

        await _prophecyRepository.SaveChangesAsync(ct);

        return ProphecyDto.FromEntity(prophecy);
    }
}