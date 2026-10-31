using MediatR;
using RccgHopeHouse.Application.Features.Announcements.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Announcements.Commands;

public record UpdateAnnouncementCommand(
    Guid Id,
    string Title,
    AnnouncementType Type,
    DateTime? StartsAt,
    DateTime? EndsAt,
    string? Summary,
    string? Body,
    string? LinkUrl,
    string? LinkText,
    int DisplayOrder,
    bool IsPublished,
    bool ShowInBanner,
    DateTime? EventStartsAt,
    DateTime? EventEndsAt,
    string? VenueName,
    string? VenueAddress
) : IRequest<AnnouncementDto>, IAnnouncementFields;

public class UpdateAnnouncementCommandHandler
    : IRequestHandler<UpdateAnnouncementCommand, AnnouncementDto>
{
    private readonly IAnnouncementRepository _repository;

    public UpdateAnnouncementCommandHandler(
        IAnnouncementRepository repository)
    {
        _repository = repository;
    }

    public async Task<AnnouncementDto> Handle(
        UpdateAnnouncementCommand request,
        CancellationToken ct)
    {
        var announcement =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Announcement), request.Id);

        announcement.Update(
            request.Title,
            request.Type,
            request.StartsAt ?? announcement.StartsAt,
            request.EndsAt,
            request.Summary,
            request.Body,
            request.LinkUrl,
            request.LinkText,
            request.DisplayOrder,
            request.IsPublished,
            request.ShowInBanner,
            request.EventStartsAt,
            request.EventEndsAt,
            request.VenueName,
            request.VenueAddress);

        await _repository.UpdateAsync(announcement, ct);
        await _repository.SaveChangesAsync(ct);

        return announcement.ToDto();
    }
}
