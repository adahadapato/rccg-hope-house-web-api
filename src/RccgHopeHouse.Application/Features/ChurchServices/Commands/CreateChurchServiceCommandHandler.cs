using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Handles the creation of a new church service.
/// </summary>
/// <remarks>
/// The handler creates the service domain entity, applies its
/// scheduling and display configuration, persists it through the
/// church service repository, and returns the resulting service DTO.
/// </remarks>
public sealed class CreateChurchServiceCommandHandler
    : IRequestHandler<
        CreateChurchServiceCommand,
        ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="CreateChurchServiceCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to persist church services.
    /// </param>
    public CreateChurchServiceCommandHandler(
        IChurchServiceRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Creates and persists a new church service.
    /// </summary>
    /// <param name="request">
    /// The command containing the church service details.
    /// </param>
    /// <param name="ct">
    /// Token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A DTO representing the newly created church service.
    /// </returns>
    public async Task<ChurchServiceDto> Handle(
        CreateChurchServiceCommand request,
        CancellationToken ct)
    {
        var service = ChurchService.Create(
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

        await _repository.AddAsync(
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
    /// The church service to map.
    /// </param>
    /// <returns>
    /// A DTO containing the persisted church service information.
    /// </returns>
    private static ChurchServiceDto MapToDto(
        ChurchService service)
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