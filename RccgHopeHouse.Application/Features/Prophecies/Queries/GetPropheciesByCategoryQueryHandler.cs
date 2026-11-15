using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Prophecies.Queries;

public class GetPropheciesByCategoryQueryHandler
    : IRequestHandler<
        GetPropheciesByCategoryQuery,
        IReadOnlyList<ProphecyDto>>
{
    private readonly IProphecyRepository _prophecyRepository;
    private readonly IProphecyCategoryRepository _categoryRepository;

    public GetPropheciesByCategoryQueryHandler(
        IProphecyRepository prophecyRepository,
        IProphecyCategoryRepository categoryRepository)
    {
        _prophecyRepository = prophecyRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<ProphecyDto>> Handle(
        GetPropheciesByCategoryQuery request,
        CancellationToken ct)
    {
        _ = await _categoryRepository.GetByIdAsync(
                request.CategoryId,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyCategory),
                request.CategoryId);

        var prophecies =
            await _prophecyRepository.GetByCategoryAsync(
                request.CategoryId,
                request.ActiveOnly,
                ct);

        return prophecies
            .Select(ProphecyDto.FromEntity)
            .ToList();
    }
}