using MediatR;
using RccgHopeHouse.Application.Features.BirthdayWishes.Dtos;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.BirthdayWishes.Queries;

/// <summary>Public: approved wishes for one celebrant photo.</summary>
public record GetBirthdayWishesForImageQuery(
    Guid GalleryImageId,
    int Take = 100)
    : IRequest<IReadOnlyList<BirthdayWishDto>>;

public class GetBirthdayWishesForImageQueryHandler
    : IRequestHandler<GetBirthdayWishesForImageQuery, IReadOnlyList<BirthdayWishDto>>
{
    private readonly IGalleryImageCommentRepository _repository;

    public GetBirthdayWishesForImageQueryHandler(IGalleryImageCommentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BirthdayWishDto>> Handle(
        GetBirthdayWishesForImageQuery request,
        CancellationToken ct)
    {
        var items = await _repository.GetApprovedForImageAsync(
            request.GalleryImageId,
            Math.Clamp(request.Take, 1, 200),
            ct);

        return items.Select(w => w.ToDto()).ToList();
    }
}

/// <summary>Public: approved wish counts for the gallery cards.</summary>
public record GetBirthdayWishCountsQuery
    : IRequest<IReadOnlyList<BirthdayWishCountDto>>;

public class GetBirthdayWishCountsQueryHandler
    : IRequestHandler<GetBirthdayWishCountsQuery, IReadOnlyList<BirthdayWishCountDto>>
{
    private readonly IGalleryImageCommentRepository _repository;

    public GetBirthdayWishCountsQueryHandler(IGalleryImageCommentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BirthdayWishCountDto>> Handle(
        GetBirthdayWishCountsQuery request,
        CancellationToken ct)
    {
        var counts = await _repository.GetApprovedCountsAsync(ct);

        return counts
            .Select(pair => new BirthdayWishCountDto(pair.Key, pair.Value))
            .ToList();
    }
}

/// <summary>Admin: wishes, optionally filtered by status.</summary>
public record GetAdminBirthdayWishesQuery(
    BirthdayWishStatus? Status = null,
    int Skip = 0,
    int Take = 100)
    : IRequest<IReadOnlyList<AdminBirthdayWishDto>>;

public class GetAdminBirthdayWishesQueryHandler
    : IRequestHandler<GetAdminBirthdayWishesQuery, IReadOnlyList<AdminBirthdayWishDto>>
{
    private readonly IGalleryImageCommentRepository _repository;

    public GetAdminBirthdayWishesQueryHandler(IGalleryImageCommentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AdminBirthdayWishDto>> Handle(
        GetAdminBirthdayWishesQuery request,
        CancellationToken ct)
    {
        var items = await _repository.GetAllAsync(
            request.Status,
            Math.Max(0, request.Skip),
            Math.Clamp(request.Take, 1, 500),
            ct);

        return items.Select(w => w.ToAdminDto()).ToList();
    }
}

/// <summary>Admin: counts per status (tabs and bell icon).</summary>
public record GetBirthdayWishStatsQuery : IRequest<BirthdayWishStatsDto>;

public class GetBirthdayWishStatsQueryHandler
    : IRequestHandler<GetBirthdayWishStatsQuery, BirthdayWishStatsDto>
{
    private readonly IGalleryImageCommentRepository _repository;

    public GetBirthdayWishStatsQueryHandler(IGalleryImageCommentRepository repository)
    {
        _repository = repository;
    }

    public async Task<BirthdayWishStatsDto> Handle(
        GetBirthdayWishStatsQuery request,
        CancellationToken ct)
    {
        // Run one after another: a DbContext cannot run queries in parallel.
        var pending = await _repository.CountAsync(BirthdayWishStatus.Pending, ct);
        var approved = await _repository.CountAsync(BirthdayWishStatus.Approved, ct);
        var rejected = await _repository.CountAsync(BirthdayWishStatus.Rejected, ct);

        return new BirthdayWishStatsDto(pending, approved, rejected);
    }
}
