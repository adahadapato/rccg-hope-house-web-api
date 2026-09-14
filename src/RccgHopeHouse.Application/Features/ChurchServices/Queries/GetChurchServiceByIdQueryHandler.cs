using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Queries;

/// <summary>
/// Handler for single service retrieval.
/// </summary>
public class GetChurchServiceByIdQueryHandler : IRequestHandler<GetChurchServiceByIdQuery, ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    public GetChurchServiceByIdQueryHandler(IChurchServiceRepository repository) => _repository = repository;

    public async Task<ChurchServiceDto> Handle(GetChurchServiceByIdQuery request, CancellationToken ct)
    {
        var service = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ChurchService), request.Id);

        return new ChurchServiceDto(
            service.Id, service.Name, service.Category, service.DayOfWeek, service.StartTime, service.EndTime,
            service.Description, service.Location, service.ZoomId, service.ZoomPasscode, service.Recurrence,
            service.DayOfMonth, service.IsActive, service.DisplayOrder);
    }
}