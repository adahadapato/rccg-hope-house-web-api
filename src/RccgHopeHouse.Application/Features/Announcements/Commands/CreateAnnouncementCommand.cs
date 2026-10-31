using MediatR;
using RccgHopeHouse.Application.Features.Announcements.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Announcements.Commands;

public record CreateAnnouncementCommand(
    string Title,
    AnnouncementType Type,
    DateTime? StartsAt,
    DateTime? EndsAt,
    string? Summary = null,
    string? Body = null,
    string? LinkUrl = null,
    string? LinkText = null,
    int DisplayOrder = 0,
    bool IsPublished = true,
    bool ShowInBanner = true,
    DateTime? EventStartsAt = null,
    DateTime? EventEndsAt = null,
    string? VenueName = null,
    string? VenueAddress = null
) : IRequest<AnnouncementDto>, IAnnouncementFields;

public class CreateAnnouncementCommandHandler
    : IRequestHandler<CreateAnnouncementCommand, AnnouncementDto>
{
    private readonly IAnnouncementRepository _repository;

    public CreateAnnouncementCommandHandler(
        IAnnouncementRepository repository)
    {
        _repository = repository;
    }

    public async Task<AnnouncementDto> Handle(
        CreateAnnouncementCommand request,
        CancellationToken ct)
    {
        var announcement = Announcement.Create(
            request.Title,
            request.Type,
            request.StartsAt ?? DateTime.UtcNow,
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

        await _repository.AddAsync(announcement, ct);
        await _repository.SaveChangesAsync(ct);

        return announcement.ToDto();
    }
}
