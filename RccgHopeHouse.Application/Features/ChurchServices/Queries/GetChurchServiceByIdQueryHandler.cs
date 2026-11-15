using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Queries;

/// <summary>
/// Handles requests to retrieve a single church service
/// by its unique identifier.
/// </summary>
/// <remarks>
/// The handler retrieves the church service from the repository
/// and maps the domain entity to a detailed
/// <see cref="ChurchServiceDto"/>.
///
/// The returned DTO includes scheduling information, recurrence,
/// location and Zoom details, presentation settings, broadcast
/// configuration and the theme for the upcoming or currently
/// occurring service.
/// </remarks>
public sealed class GetChurchServiceByIdQueryHandler
    : IRequestHandler<
        GetChurchServiceByIdQuery,
        ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetChurchServiceByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve church service records.
    /// </param>
    public GetChurchServiceByIdQueryHandler(
        IChurchServiceRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves the requested church service and maps it
    /// to a detailed church service DTO.
    /// </summary>
    /// <param name="request">
    /// The query containing the unique identifier of the
    /// church service to retrieve.
    /// </param>
    /// <param name="ct">
    /// Token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A DTO containing the requested church service information.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when no church service exists with the supplied identifier.
    /// </exception>
    public async Task<ChurchServiceDto> Handle(
        GetChurchServiceByIdQuery request,
        CancellationToken ct)
    {
        var service =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(Core.Entities.ChurchService),
                request.Id);

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