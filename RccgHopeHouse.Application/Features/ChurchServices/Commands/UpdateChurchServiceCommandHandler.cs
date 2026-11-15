using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Handles updates to an existing church service.
/// </summary>
/// <remarks>
/// The handler retrieves the existing service, applies its core,
/// scheduling, display, broadcast and current-theme settings,
/// persists the changes, and returns the updated service DTO.
/// </remarks>
public sealed class UpdateChurchServiceCommandHandler
    : IRequestHandler<
        UpdateChurchServiceCommand,
        ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="UpdateChurchServiceCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve and persist church services.
    /// </param>
    public UpdateChurchServiceCommandHandler(
        IChurchServiceRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Updates an existing church service.
    /// </summary>
    /// <param name="request">
    /// The command containing the updated church service details.
    /// </param>
    /// <param name="ct">
    /// Token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A DTO representing the updated church service.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the requested church service cannot be found.
    /// </exception>
    public async Task<ChurchServiceDto> Handle(
        UpdateChurchServiceCommand request,
        CancellationToken ct)
    {
        var service =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(Core.Entities.ChurchService),
                request.Id);

        service.Update(
            request.Name,
            request.Category,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.Description,
            request.Recurrence,
            request.DayOfMonth,
            request.IsLocal,
            request.Icon,
            request.ShowInMonthlyServices,
            request.IsBroadcastEnabled,
            request.CurrentTheme);

        service.UpdateSchedule(
            request.StartTime,
            request.EndTime,
            request.Location,
            request.ZoomId,
            request.ZoomPasscode);

        service.SetDisplayOrder(
            request.DisplayOrder);

        await _repository.UpdateAsync(
            service,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        return MapToDto(service);
    }

    /// <summary>
    /// Maps a church service domain entity to its application DTO.
    /// </summary>
    /// <param name="service">
    /// The updated church service.
    /// </param>
    /// <returns>
    /// A DTO containing the updated church service information.
    /// </returns>
    private static ChurchServiceDto MapToDto(
        Core.Entities.ChurchService service)
    {
        return new ChurchServiceDto(
            service.Id,
            service.Name,
            service.Category,
            service.DayOfWeek,
            service.StartTime,
            service.EndTime,
            service.Description,
            service.Location,
            service.ZoomId,
            service.ZoomPasscode,
            service.Recurrence,
            service.DayOfMonth,
            service.IsLocal,
            service.IsActive,
            service.DisplayOrder,
            service.Icon,
            service.ShowInMonthlyServices,
            service.IsBroadcastEnabled,
            service.CurrentTheme);
    }
}