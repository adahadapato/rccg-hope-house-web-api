using MediatR;
using RccgHopeHouse.Application.Features.ChurchEvents.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Commands;

public class CreateChurchEventCommandHandler
    : IRequestHandler<
        CreateChurchEventCommand,
        ChurchEventDto>
{
    private readonly IChurchEventRepository _repository;

    public CreateChurchEventCommandHandler(
        IChurchEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChurchEventDto> Handle(
        CreateChurchEventCommand request,
        CancellationToken ct)
    {
        var churchEvent = ChurchEvent.Create(
            request.Title,
            request.Category,
            request.StartDateTime,
            request.EndDateTime,
            request.Description,
            request.Location,
            request.Icon,
            request.Color,
            request.RegistrationUrl,
            request.RegistrationButtonText,
            request.ImageUrl,
            request.DisplayOrder,
            request.IsActive);

        await _repository.AddAsync(churchEvent, ct);

        await _repository.SaveChangesAsync(ct);

        return MapToDto(churchEvent);
    }

    private static ChurchEventDto MapToDto(ChurchEvent churchEvent)
    {
        return new ChurchEventDto(
            churchEvent.Id,
            churchEvent.Title,
            churchEvent.Category,
            churchEvent.StartDateTime,
            churchEvent.EndDateTime,
            churchEvent.Description,
            churchEvent.Location,
            churchEvent.Icon,
            churchEvent.Color,
            churchEvent.RegistrationUrl,
            churchEvent.RegistrationButtonText,
            churchEvent.ImageUrl,
            churchEvent.IsActive,
            churchEvent.DisplayOrder);
    }
}