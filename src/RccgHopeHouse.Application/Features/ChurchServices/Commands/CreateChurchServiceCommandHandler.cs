using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

public class CreateChurchServiceCommandHandler
    : IRequestHandler<CreateChurchServiceCommand, ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    public CreateChurchServiceCommandHandler(
        IChurchServiceRepository repository)
    {
        _repository = repository;
    }

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
            request.ShowInMonthlyServices);

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
            service.ShowInMonthlyServices);
    }
}