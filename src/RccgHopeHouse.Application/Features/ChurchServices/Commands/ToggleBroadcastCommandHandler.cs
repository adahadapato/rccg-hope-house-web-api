using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Handles requests to enable or disable broadcasting for
/// a church service.
/// </summary>
/// <remarks>
/// The handler retrieves the requested church service, toggles
/// its broadcast-enabled state using the domain entity's
/// <c>SetBroadcastEnabled</c> method, persists the change and
/// returns the updated service information.
///
/// Broadcast enablement is independent of the service's active state,
/// monthly-service visibility and current theme.
/// </remarks>
public sealed class ToggleBroadcastCommandHandler
    : IRequestHandler<
        ToggleBroadcastCommand,
        ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ToggleBroadcastCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve and persist church services.
    /// </param>
    public ToggleBroadcastCommandHandler(
        IChurchServiceRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Toggles whether broadcasts are enabled for the requested
    /// church service.
    /// </summary>
    /// <param name="request">
    /// The command containing the identifier of the church service
    /// whose broadcast state should be toggled.
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
        ToggleBroadcastCommand request,
        CancellationToken ct)
    {
        var service =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(Core.Entities.ChurchService),
                request.Id);

        service.SetBroadcastEnabled(
            !service.IsBroadcastEnabled);

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