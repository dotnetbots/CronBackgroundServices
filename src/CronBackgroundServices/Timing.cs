using Cronos;

namespace CronBackgroundServices;

internal class Timing(string timeZoneId)
{
    public readonly TimeZoneInfo TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

    public DateTimeOffset RelativeNow(DateTimeOffset? nowutc = null)
    {
        return TimeZoneInfo.ConvertTime(nowutc ?? DateTimeOffset.UtcNow, TimeZoneInfo);
    }

    public DateTimeOffset? GetNextOccurenceInRelativeTime(string cron)
    {
        var expression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
        return expression.GetNextOccurrence(DateTimeOffset.UtcNow, TimeZoneInfo);
    }

    public IEnumerable<string> Get10NextOccurrences(string cron)
    {
        var expression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
        var fromUtc = DateTime.UtcNow;
        var upcoming = new List<DateTime>();
        upcoming.AddRange(GetTenNextsOccurrences(upcoming, expression, fromUtc, fromUtc.AddMonths(1)));
        return upcoming.Select(u => $"{u.ToLongDateString()} {u.ToLongTimeString()}");
    }

    private IEnumerable<DateTime> GetTenNextsOccurrences(List<DateTime> upcoming, CronExpression expression,
        DateTime fromUtc,
        DateTime toUtc)
    {
        while (true)
        {
            toUtc = toUtc.AddMonths(1);
            var occurrences = expression.GetOccurrences(fromUtc, toUtc);
            upcoming = occurrences.ToList();

            if (upcoming.Count < 10)
            {
                continue;
            }

            break;
        }

        return upcoming.Take(10);
    }
}
