using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

public class ToggleBroadcastCommandHandler
    : IRequestHandler<ToggleBroadcastCommand, ChurchServiceDto>
{
    private readonly IChurchServiceRepository _repository;

    public ToggleBroadcastCommandHandler(IChurchServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChurchServiceDto> Handle(
        ToggleBroadcastCommand request, CancellationToken ct)
    {
        var service = await _repository.GetByIdAsync(request.Id,ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ChurchService), request.Id);

        service.SetBroadcastEnabled(!service.IsBroadcastEnabled);

        await _repository.UpdateAsync(service, ct);

        await _repository.SaveChangesAsync(ct);

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
            ShowInMonthlyServices: service.ShowInMonthlyServices,
            IsBroadcastEnabled: service.IsBroadcastEnabled);
    }
}