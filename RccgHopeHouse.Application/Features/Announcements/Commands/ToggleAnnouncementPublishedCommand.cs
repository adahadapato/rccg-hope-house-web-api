using MediatR;
using RccgHopeHouse.Application.Features.Announcements.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Announcements.Commands;

public record ToggleAnnouncementPublishedCommand(Guid Id)
    : IRequest<AnnouncementDto>;

public class ToggleAnnouncementPublishedCommandHandler
    : IRequestHandler<ToggleAnnouncementPublishedCommand, AnnouncementDto>
{
    private readonly IAnnouncementRepository _repository;

    public ToggleAnnouncementPublishedCommandHandler(
        IAnnouncementRepository repository)
    {
        _repository = repository;
    }

    public async Task<AnnouncementDto> Handle(
        ToggleAnnouncementPublishedCommand request,
        CancellationToken ct)
    {
        var announcement =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Announcement), request.Id);

        announcement.TogglePublished();

        await _repository.UpdateAsync(announcement, ct);
        await _repository.SaveChangesAsync(ct);

        return announcement.ToDto();
    }
}
