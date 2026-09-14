using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Handler for toggling service visibility.
/// </summary>
public class ToggleActiveCommandHandler : IRequestHandler<ToggleActiveCommand, ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    public ToggleActiveCommandHandler(IChurchServiceRepository repository) => _repository = repository;

    public async Task<ChurchServiceDto> Handle(ToggleActiveCommand request, CancellationToken ct)
    {
        var service = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ChurchService), request.Id);

        service.ToggleActive();

        await _repository.UpdateAsync(service, ct);
        await _repository.SaveChangesAsync(ct);

        return new ChurchServiceDto(
            service.Id, service.Name, service.Category, service.DayOfWeek, service.StartTime, service.EndTime,
            service.Description, service.Location, service.ZoomId, service.ZoomPasscode, service.Recurrence,
            service.DayOfMonth, service.IsActive, service.DisplayOrder);
    }
}