using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.SiteVisits.Commands;

/// <summary>
/// Sent by the public website. With <c>Seconds</c> null it counts a
/// page view; otherwise it adds reading time to the visitor's day.
/// </summary>
public record RecordSiteVisitCommand(
    DateOnly VisitDate,
    string VisitorHash,
    bool IsMobile,
    int? Seconds)
    : IRequest;

public sealed class RecordSiteVisitCommandHandler
    : IRequestHandler<RecordSiteVisitCommand>
{
    private readonly ISiteVisitRepository _repository;

    public RecordSiteVisitCommandHandler(ISiteVisitRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        RecordSiteVisitCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var now = DateTime.UtcNow;

        if (request.Seconds is null)
        {
            await _repository.RecordPageViewAsync(
                request.VisitDate,
                request.VisitorHash,
                request.IsMobile,
                now,
                ct);

            return;
        }

        var seconds = Math.Clamp(
            request.Seconds.Value,
            0,
            SiteVisit.MaxSecondsPerReport);

        await _repository.AddEngagedSecondsAsync(
            request.VisitDate,
            request.VisitorHash,
            seconds,
            now,
            ct);
    }
}
