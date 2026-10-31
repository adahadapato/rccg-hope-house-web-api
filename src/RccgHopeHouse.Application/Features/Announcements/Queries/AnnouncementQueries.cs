using MediatR;
using RccgHopeHouse.Application.Features.Announcements.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Announcements.Queries;

/// <summary>Admin list of all announcements.</summary>
public record GetAnnouncementsQuery(
    bool? IsPublished = null,
    int Skip = 0,
    int Take = 100)
    : IRequest<IReadOnlyList<AnnouncementDto>>;

public class GetAnnouncementsQueryHandler
    : IRequestHandler<GetAnnouncementsQuery, IReadOnlyList<AnnouncementDto>>
{
    private readonly IAnnouncementRepository _repository;

    public GetAnnouncementsQueryHandler(IAnnouncementRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AnnouncementDto>> Handle(
        GetAnnouncementsQuery request,
        CancellationToken ct)
    {
        var items = await _repository.GetAllAsync(
            request.IsPublished,
            Math.Max(0, request.Skip),
            Math.Clamp(request.Take, 1, 500),
            ct);

        return items.Select(a => a.ToDto()).ToList();
    }
}

public record GetAnnouncementByIdQuery(Guid Id)
    : IRequest<AnnouncementDto>;

public class GetAnnouncementByIdQueryHandler
    : IRequestHandler<GetAnnouncementByIdQuery, AnnouncementDto>
{
    private readonly IAnnouncementRepository _repository;

    public GetAnnouncementByIdQueryHandler(IAnnouncementRepository repository)
    {
        _repository = repository;
    }

    public async Task<AnnouncementDto> Handle(
        GetAnnouncementByIdQuery request,
        CancellationToken ct)
    {
        var announcement =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Announcement), request.Id);

        return announcement.ToDto();
    }
}

/// <summary>
/// Public feed of live announcements for the floating banner.
/// </summary>
public record GetLiveAnnouncementsQuery(
    bool BannerOnly = false,
    int Take = 10)
    : IRequest<IReadOnlyList<AnnouncementFeedDto>>;

public class GetLiveAnnouncementsQueryHandler
    : IRequestHandler<GetLiveAnnouncementsQuery, IReadOnlyList<AnnouncementFeedDto>>
{
    private readonly IAnnouncementRepository _repository;

    public GetLiveAnnouncementsQueryHandler(IAnnouncementRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AnnouncementFeedDto>> Handle(
        GetLiveAnnouncementsQuery request,
        CancellationToken ct)
    {
        var items = await _repository.GetLiveAsync(
            DateTime.UtcNow,
            request.BannerOnly,
            Math.Clamp(request.Take, 1, 20),
            ct);

        return items.Select(a => a.ToFeedDto()).ToList();
    }
}
