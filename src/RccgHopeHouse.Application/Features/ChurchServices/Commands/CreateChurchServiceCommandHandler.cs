using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Handler for creating a new church service.
/// Orchestrates domain creation, schedule configuration, and persistence.
/// </summary>
public class CreateChurchServiceCommandHandler : IRequestHandler<CreateChurchServiceCommand, ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    public CreateChurchServiceCommandHandler(IChurchServiceRepository repository) => _repository = repository;

    /// <summary>
    /// Creates the service entity, applies schedule/location settings, and persists.
    /// </summary>
    public async Task<ChurchServiceDto> Handle(CreateChurchServiceCommand request, CancellationToken ct)
    {
        // Domain factory creates base entity
        var service = ChurchService.Create(
            request.Name,
            request.Category,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.Description,
            request.Recurrence,
            request.DayOfMonth);

        // Apply location & Zoom details if provided
        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            service.UpdateSchedule(request.StartTime, request.EndTime, request.Location, request.ZoomId, request.ZoomPasscode);
        }

        service.SetDisplayOrder(request.DisplayOrder);

        await _repository.AddAsync(service, ct);
        await _repository.SaveChangesAsync(ct);

        return MapToDto(service);
    }

    private static ChurchServiceDto MapToDto(ChurchService s) => new(
        s.Id, s.Name, s.Category, s.DayOfWeek, s.StartTime, s.EndTime,
        s.Description, s.Location, s.ZoomId, s.ZoomPasscode, s.Recurrence,
        s.DayOfMonth, s.IsActive, s.DisplayOrder);
}