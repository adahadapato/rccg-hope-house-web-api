using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Queries;

public class GetProphecyCategoriesByYearQueryHandler
    : IRequestHandler<
        GetProphecyCategoriesByYearQuery,
        IReadOnlyList<ProphecyCategoryDto>>
{
    private readonly IProphecyCategoryRepository _categoryRepository;
    private readonly IProphecyYearRepository _yearRepository;

    public GetProphecyCategoriesByYearQueryHandler(
        IProphecyCategoryRepository categoryRepository,
        IProphecyYearRepository yearRepository)
    {
        _categoryRepository = categoryRepository;
        _yearRepository = yearRepository;
    }

    public async Task<IReadOnlyList<ProphecyCategoryDto>> Handle(
        GetProphecyCategoriesByYearQuery request,
        CancellationToken ct)
    {
        _ = await _yearRepository.GetByIdAsync(
                request.ProphecyYearId,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyYear),
                request.ProphecyYearId);

        var categories =
            await _categoryRepository.GetByYearAsync(
                request.ProphecyYearId,
                request.ActiveOnly,
                ct);

        return categories
            .Select(ProphecyCategoryDto.FromEntity)
            .ToList();
    }
}