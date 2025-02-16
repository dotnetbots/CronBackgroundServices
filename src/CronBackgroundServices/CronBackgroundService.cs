using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CronBackgroundServices;

internal class CronBackgroundService(IRecurringAction Action, ILogger logger) : BackgroundService
{
    private readonly Timing _timing = new(Action.GetTimeZoneId());

    private string Cron { get; } = Action.Cron;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogTrace(
            $"Using {Cron} and timezone '{_timing.TimeZoneInfo.Id}. The time in this timezone: {_timing.RelativeNow()}'");
        DateTimeOffset? next = null;

        do
        {
            var now = _timing.RelativeNow();

            if (next == null)
            {
                next = _timing.GetNextOccurenceInRelativeTime(Cron);
                var uText = _timing.Get10NextOccurrences(Cron);
                var logText = $"Ten next occurrences :\n{uText.Aggregate((x, y) => x + "\n" + y)}";
                logger.LogTrace(logText);
            }

            if (now > next)
            {
                try
                {
                    await Action.Process(stoppingToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                }

                next = _timing.GetNextOccurenceInRelativeTime(Cron);
                logger.LogTrace(next is not null
                    ? $"Next at {next.Value.DateTime.ToLongDateString()} {next.Value.DateTime.ToLongTimeString()}"
                    : "No more occurences.");
            }
            else
            {
                // needed for graceful shutdown for some reason.
                // 100ms chosen so it doesn't affect calculating the next
                // cron occurence (lowest possible: every second)
                await Task.Delay(100);
            }
        } while (!stoppingToken.IsCancellationRequested);
    }
}
