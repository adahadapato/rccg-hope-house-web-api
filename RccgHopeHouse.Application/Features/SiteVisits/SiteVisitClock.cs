namespace RccgHopeHouse.Application.Features.SiteVisits;

/// <summary>
/// Website visits are counted by the UK calendar day, so that
/// "today" on the dashboard matches the church's day whatever
/// time zone the server runs in.
/// </summary>
public static class SiteVisitClock
{
    private static readonly TimeZoneInfo UkTimeZone = FindUkTimeZone();

    public static DateOnly Today()
    {
        return DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, UkTimeZone));
    }

    private static TimeZoneInfo FindUkTimeZone()
    {
        // The first name is used on Linux and modern Windows,
        // the second on older Windows servers.
        foreach (var id in new[] { "Europe/London", "GMT Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (Exception ex) when (
                ex is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                // Try the next name.
            }
        }

        return TimeZoneInfo.Utc;
    }
}
