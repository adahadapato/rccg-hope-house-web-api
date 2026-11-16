using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

public class SiteVisitRepository : ISiteVisitRepository
{
    private readonly ApplicationDbContext _context;

    public SiteVisitRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RecordPageViewAsync(
        DateOnly visitDate,
        string visitorHash,
        bool isMobile,
        DateTime nowUtc,
        CancellationToken ct = default)
    {
        // The common case: the visitor already has a row for today.
        // The count is increased inside the database, so two pages
        // opened at the same moment are both counted.
        if (await AddPageViewAsync(visitDate, visitorHash, nowUtc, ct) > 0)
        {
            return;
        }

        var visit = SiteVisit.Start(visitDate, visitorHash, isMobile, nowUtc);

        _context.SiteVisits.Add(visit);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Another request created the row a moment earlier
            // (the unique index stopped a duplicate). Count this
            // page view on that row instead.
            _context.Entry(visit).State = EntityState.Detached;

            await AddPageViewAsync(visitDate, visitorHash, nowUtc, ct);
        }
    }

    public async Task AddEngagedSecondsAsync(
        DateOnly visitDate,
        string visitorHash,
        int seconds,
        DateTime nowUtc,
        CancellationToken ct = default)
    {
        if (seconds <= 0)
        {
            return;
        }

        var maxPerDay = SiteVisit.MaxSecondsPerDay;

        await _context
            .SiteVisits
            .Where(v =>
                v.VisitDate == visitDate &&
                v.VisitorHash == visitorHash)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        v => v.EngagedSeconds,
                        v => v.EngagedSeconds + seconds > maxPerDay
                            ? maxPerDay
                            : v.EngagedSeconds + seconds)
                    .SetProperty(v => v.LastSeenAt, nowUtc),
                ct);
    }

    public async Task<IReadOnlyList<SiteVisitDayTotals>> GetDailyTotalsAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default)
    {
        var rows = await _context
            .SiteVisits
            .AsNoTracking()
            .Where(v => v.VisitDate >= from && v.VisitDate <= to)
            .GroupBy(v => v.VisitDate)
            .Select(g => new
            {
                Date = g.Key,
                Visitors = g.Count(),
                PageViews = g.Sum(v => v.PageViews),
                EngagedSeconds = g.Sum(v => v.EngagedSeconds),
                MobileVisitors = g.Sum(v => v.IsMobile ? 1 : 0)
            })
            .OrderBy(r => r.Date)
            .ToListAsync(ct);

        return rows
            .Select(r => new SiteVisitDayTotals(
                r.Date,
                r.Visitors,
                r.PageViews,
                r.EngagedSeconds,
                r.MobileVisitors))
            .ToList();
    }

    public async Task<int> GetActiveCountAsync(
        DateTime sinceUtc,
        CancellationToken ct = default)
    {
        return await _context
            .SiteVisits
            .AsNoTracking()
            .CountAsync(v => v.LastSeenAt >= sinceUtc, ct);
    }

    public async Task<DateOnly?> GetFirstVisitDateAsync(
        CancellationToken ct = default)
    {
        return await _context
            .SiteVisits
            .AsNoTracking()
            .OrderBy(v => v.VisitDate)
            .Select(v => (DateOnly?)v.VisitDate)
            .FirstOrDefaultAsync(ct);
    }

    private async Task<int> AddPageViewAsync(
        DateOnly visitDate,
        string visitorHash,
        DateTime nowUtc,
        CancellationToken ct)
    {
        return await _context
            .SiteVisits
            .Where(v =>
                v.VisitDate == visitDate &&
                v.VisitorHash == visitorHash)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(v => v.PageViews, v => v.PageViews + 1)
                    .SetProperty(v => v.LastSeenAt, nowUtc),
                ct);
    }
}
