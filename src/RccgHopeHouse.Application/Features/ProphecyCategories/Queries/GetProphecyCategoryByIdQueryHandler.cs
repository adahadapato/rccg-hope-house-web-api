using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Queries;

public class GetProphecyCategoryByIdQueryHandler
    : IRequestHandler<
        GetProphecyCategoryByIdQuery,
        ProphecyCategoryDto>
{
    private readonly IProphecyCategoryRepository _repository;

    public GetProphecyCategoryByIdQueryHandler(
        IProphecyCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProphecyCategoryDto> Handle(
        GetProphecyCategoryByIdQuery request,
        CancellationToken ct)
    {
        var category =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(ProphecyCategory),
                request.Id);

        return ProphecyCategoryDto.FromEntity(
            category);
    }
}