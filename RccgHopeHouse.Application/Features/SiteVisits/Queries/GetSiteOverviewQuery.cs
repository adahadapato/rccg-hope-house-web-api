using MediatR;
using RccgHopeHouse.Application.Features.SiteVisits.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.SiteVisits.Queries;

/// <summary>
/// Website figures for the last <c>Days</c> days (today included),
/// compared with the same number of days before that.
/// </summary>
public record GetSiteOverviewQuery(int Days = 30)
    : IRequest<SiteOverviewDto>;

public sealed class GetSiteOverviewQueryHandler
    : IRequestHandler<GetSiteOverviewQuery, SiteOverviewDto>
{
    private const int MaxDays = 365;

    /// <summary>A visitor counts as "online now" for five minutes.</summary>
    private static readonly TimeSpan ActiveWindow = TimeSpan.FromMinutes(5);

    private readonly ISiteVisitRepository _repository;

    public GetSiteOverviewQueryHandler(ISiteVisitRepository repository)
    {
        _repository = repository;
    }

    public async Task<SiteOverviewDto> Handle(
        GetSiteOverviewQuery request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var days = Math.Clamp(request.Days, 1, MaxDays);

        var to = SiteVisitClock.Today();
        var from = to.AddDays(-(days - 1));
        var previousFrom = from.AddDays(-days);

        // One query covers this period and the one before it.
        var totals = await _repository.GetDailyTotalsAsync(previousFrom, to, ct);

        var current = totals.Where(t => t.Date >= from).ToList();
        var previous = totals.Where(t => t.Date < from).ToList();

        var visitors = current.Sum(t => t.Visitors);
        var pageViews = current.Sum(t => t.PageViews);
        var averageSeconds = Average(current);
        var mobilePercent = MobilePercent(current);

        var previousVisitors = previous.Sum(t => t.Visitors);
        var previousPageViews = previous.Sum(t => t.PageViews);
        var previousAverage = Average(previous);

        var byDate = current.ToDictionary(t => t.Date);

        var daily = Enumerable
            .Range(0, days)
            .Select(offset => from.AddDays(offset))
            .Select(date => byDate.TryGetValue(date, out var day)
                ? new SiteOverviewDayDto(date, day.Visitors, day.PageViews)
                : new SiteOverviewDayDto(date, 0, 0))
            .ToList();

        return new SiteOverviewDto(
            Days: days,
            Visitors: visitors,
            PageViews: pageViews,
            AverageSeconds: averageSeconds,
            MobilePercent: mobilePercent,
            VisitorsChangePercent: Change(visitors, previousVisitors),
            PageViewsChangePercent: Change(pageViews, previousPageViews),
            AverageSecondsChangePercent: Change(averageSeconds, previousAverage),
            MobileChangePoints: previousVisitors > 0
                ? mobilePercent - MobilePercent(previous)
                : null,
            ActiveNow: await _repository.GetActiveCountAsync(
                DateTime.UtcNow - ActiveWindow, ct),
            CountingSince: await _repository.GetFirstVisitDateAsync(ct),
            Daily: daily);
    }

    private static int Average(IReadOnlyCollection<SiteVisitDayTotals> days)
    {
        var visitors = days.Sum(t => t.Visitors);

        return visitors == 0
            ? 0
            : (int)Math.Round(
                days.Sum(t => (long)t.EngagedSeconds) / (double)visitors);
    }

    private static int MobilePercent(IReadOnlyCollection<SiteVisitDayTotals> days)
    {
        var visitors = days.Sum(t => t.Visitors);

        return visitors == 0
            ? 0
            : (int)Math.Round(
                days.Sum(t => t.MobileVisitors) * 100.0 / visitors);
    }

    /// <summary>Percentage change, or null when there is no earlier figure.</summary>
    private static double? Change(int current, int previous)
    {
        return previous <= 0
            ? null
            : Math.Round((current - previous) * 100.0 / previous, 1);
    }
}
