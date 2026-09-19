using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GivingTypes.Queries;

/// <summary>
/// Handles retrieval of active giving types.
/// </summary>
public class GetActiveGivingTypesQueryHandler
    : IRequestHandler<GetActiveGivingTypesQuery, IReadOnlyList<GivingTypeDto>>
{
    private readonly IGivingTypeRepository _repository;

    public GetActiveGivingTypesQueryHandler(
        IGivingTypeRepository repository) =>
        _repository = repository;

    public async Task<IReadOnlyList<GivingTypeDto>> Handle(
        GetActiveGivingTypesQuery request,
        CancellationToken ct)
    {
        var givingTypes = await _repository.GetActiveAsync(ct);

        return givingTypes
            .Select(GivingTypeDto.FromEntity)
            .ToList();
    }
}