using MediatR;
using RccgHopeHouse.Application.Features.FirstTimers.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.FirstTimers.Queries;

// ===== List =====

public record GetFirstTimersQuery(
    FirstTimerFollowUpStatus? Status = null,
    string? Search = null,
    int Skip = 0,
    int Take = 50)
    : IRequest<IReadOnlyList<FirstTimerDto>>;

public sealed class GetFirstTimersQueryHandler
    : IRequestHandler<GetFirstTimersQuery, IReadOnlyList<FirstTimerDto>>
{
    private const int MaxTake = 200;

    private readonly IFirstTimerRepository _repository;

    public GetFirstTimersQueryHandler(IFirstTimerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<FirstTimerDto>> Handle(
        GetFirstTimersQuery request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var firstTimers = await _repository.GetListAsync(
            request.Status,
            request.Search,
            Math.Max(request.Skip, 0),
            Math.Clamp(request.Take, 1, MaxTake),
            ct);

        return firstTimers
            .Select(FirstTimerDto.FromEntity)
            .ToList();
    }
}

// ===== One =====

public record GetFirstTimerByIdQuery(Guid Id) : IRequest<FirstTimerDto>;

public sealed class GetFirstTimerByIdQueryHandler
    : IRequestHandler<GetFirstTimerByIdQuery, FirstTimerDto>
{
    private readonly IFirstTimerRepository _repository;

    public GetFirstTimerByIdQueryHandler(IFirstTimerRepository repository)
    {
        _repository = repository;
    }

    public async Task<FirstTimerDto> Handle(
        GetFirstTimerByIdQuery request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var firstTimer =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(FirstTimer), request.Id);

        return FirstTimerDto.FromEntity(firstTimer);
    }
}

// ===== Stats =====

public record GetFirstTimerStatsQuery : IRequest<FirstTimerStatsDto>;

public sealed class GetFirstTimerStatsQueryHandler
    : IRequestHandler<GetFirstTimerStatsQuery, FirstTimerStatsDto>
{
    private readonly IFirstTimerRepository _repository;

    public GetFirstTimerStatsQueryHandler(IFirstTimerRepository repository)
    {
        _repository = repository;
    }

    public async Task<FirstTimerStatsDto> Handle(
        GetFirstTimerStatsQuery request,
        CancellationToken ct)
    {
        return new FirstTimerStatsDto(
            NotContacted: await _repository.GetCountByStatusAsync(
                FirstTimerFollowUpStatus.NotContacted, ct),
            Contacted: await _repository.GetCountByStatusAsync(
                FirstTimerFollowUpStatus.Contacted, ct),
            Visited: await _repository.GetCountByStatusAsync(
                FirstTimerFollowUpStatus.Visited, ct),
            Closed: await _repository.GetCountByStatusAsync(
                FirstTimerFollowUpStatus.Closed, ct),
            Promoted: await _repository.GetPromotedCountAsync(ct));
    }
}
