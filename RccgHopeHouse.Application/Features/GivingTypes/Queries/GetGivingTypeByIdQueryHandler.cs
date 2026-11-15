using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GivingTypes.Queries;

/// <summary>
/// Handles retrieval of a single giving type.
/// </summary>
public class GetGivingTypeByIdQueryHandler
    : IRequestHandler<GetGivingTypeByIdQuery, GivingTypeDto>
{
    private readonly IGivingTypeRepository _repository;

    public GetGivingTypeByIdQueryHandler(
        IGivingTypeRepository repository) =>
        _repository = repository;

    public async Task<GivingTypeDto> Handle(
        GetGivingTypeByIdQuery request,
        CancellationToken ct)
    {
        var givingType = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(GivingType), request.Id);

        return GivingTypeDto.FromEntity(givingType);
    }
}