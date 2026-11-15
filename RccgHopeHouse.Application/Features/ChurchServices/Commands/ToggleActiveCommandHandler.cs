using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Handles requests to toggle the active state of a church service.
/// </summary>
/// <remarks>
/// The handler retrieves the requested church service, toggles its
/// active state through the domain entity, persists the change and
/// returns the updated service information.
///
/// Toggling the active state does not modify the service's schedule,
/// broadcast configuration, monthly-service visibility or current theme.
/// </remarks>
public sealed class ToggleActiveCommandHandler
    : IRequestHandler<
        ToggleActiveCommand,
        ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ToggleActiveCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve and persist church services.
    /// </param>
    public ToggleActiveCommandHandler(
        IChurchServiceRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Toggles the active state of the requested church service.
    /// </summary>
    /// <param name="request">
    /// The command containing the identifier of the church service
    /// whose active state should be toggled.
    /// </param>
    /// <param name="ct">
    /// Token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A DTO containing the updated church service information.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when no church service exists with the supplied identifier.
    /// </exception>
    public async Task<ChurchServiceDto> Handle(
        ToggleActiveCommand request,
        CancellationToken ct)
    {
        var service =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(Core.Entities.ChurchService),
                request.Id);

        service.ToggleActive();

        await _repository.UpdateAsync(
            service,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        return MapToDto(service);
    }

    /// <summary>
    /// Maps a church service domain entity to its detailed DTO.
    /// </summary>
    /// <param name="service">
    /// The church service to map.
    /// </param>
    /// <returns>
    /// A DTO containing the current church service state.
    /// </returns>
    private static ChurchServiceDto MapToDto(
        Core.Entities.ChurchService service)
    {
        return new ChurchServiceDto(
            Id: service.Id,
            Name: service.Name,
            Category: service.Category,
            DayOfWeek: service.DayOfWeek,
            StartTime: service.StartTime,
            EndTime: service.EndTime,
            Description: service.Description,
            Location: service.Location,
            ZoomId: service.ZoomId,
            ZoomPasscode: service.ZoomPasscode,
            Recurrence: service.Recurrence,
            DayOfMonth: service.DayOfMonth,
            IsLocal: service.IsLocal,
            IsActive: service.IsActive,
            DisplayOrder: service.DisplayOrder,
            Icon: service.Icon,
            ShowInMonthlyServices:
                service.ShowInMonthlyServices,
            IsBroadcastEnabled:
                service.IsBroadcastEnabled,
            CurrentTheme:
                service.CurrentTheme);
    }
}