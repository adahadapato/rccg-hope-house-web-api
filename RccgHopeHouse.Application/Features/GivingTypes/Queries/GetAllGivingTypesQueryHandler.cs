using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GivingTypes.Queries;

/// <summary>
/// Handles retrieval of all giving types.
/// </summary>
public class GetAllGivingTypesQueryHandler
    : IRequestHandler<GetAllGivingTypesQuery, IReadOnlyList<GivingTypeDto>>
{
    private readonly IGivingTypeRepository _repository;

    public GetAllGivingTypesQueryHandler(
        IGivingTypeRepository repository) =>
        _repository = repository;

    public async Task<IReadOnlyList<GivingTypeDto>> Handle(
        GetAllGivingTypesQuery request,
        CancellationToken ct)
    {
        var givingTypes = await _repository.GetAllAsync(ct);

        return givingTypes
            .Select(GivingTypeDto.FromEntity)
            .ToList();
    }
}