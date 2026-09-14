using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Handler for updating a church service.
/// Loads existing entity, applies domain updates, and persists.
/// </summary>
public class UpdateChurchServiceCommandHandler : IRequestHandler<UpdateChurchServiceCommand, ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    public UpdateChurchServiceCommandHandler(IChurchServiceRepository repository) => _repository = repository;

    public async Task<ChurchServiceDto> Handle(UpdateChurchServiceCommand request, CancellationToken ct)
    {
        var service = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ChurchService), request.Id);

        // Update base metadata
        service.Update(
            request.Name,
            request.Category,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.Description,
            request.Recurrence,
            request.DayOfMonth);

        // Update location & Zoom if provided
        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            service.UpdateSchedule(request.StartTime, request.EndTime, request.Location, request.ZoomId, request.ZoomPasscode);
        }

        service.SetDisplayOrder(request.DisplayOrder);

        await _repository.UpdateAsync(service, ct);
        await _repository.SaveChangesAsync(ct);

        return new ChurchServiceDto(
            service.Id, service.Name, service.Category, service.DayOfWeek, service.StartTime, service.EndTime,
            service.Description, service.Location, service.ZoomId, service.ZoomPasscode, service.Recurrence,
            service.DayOfMonth, service.IsActive, service.DisplayOrder);
    }
}