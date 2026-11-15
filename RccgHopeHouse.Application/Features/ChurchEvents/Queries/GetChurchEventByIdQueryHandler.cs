using MediatR;
using RccgHopeHouse.Application.Features.ChurchEvents.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Queries;

public class GetChurchEventByIdQueryHandler
    : IRequestHandler<
        GetChurchEventByIdQuery,
        ChurchEventDto>
{
    private readonly IChurchEventRepository _repository;

    public GetChurchEventByIdQueryHandler(
        IChurchEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChurchEventDto> Handle(
        GetChurchEventByIdQuery request,
        CancellationToken ct)
    {
        var churchEvent =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(Core.Entities.ChurchEvent),
                request.Id);

        return new ChurchEventDto(
            Id: churchEvent.Id,
            Title: churchEvent.Title,
            Category: churchEvent.Category,
            StartDateTime:
                churchEvent.StartDateTime,
            EndDateTime:
                churchEvent.EndDateTime,
            Description:
                churchEvent.Description,
            Location:
                churchEvent.Location,
            Icon:
                churchEvent.Icon,
            Color:
                churchEvent.Color,
            RegistrationUrl:
                churchEvent.RegistrationUrl,
            RegistrationButtonText:
                churchEvent.RegistrationButtonText,
            ImageUrl:
                churchEvent.ImageUrl,
            IsActive:
                churchEvent.IsActive,
            DisplayOrder:
                churchEvent.DisplayOrder);
    }
}