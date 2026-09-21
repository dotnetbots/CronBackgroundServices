using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CronBackgroundServices;

internal class CronBackgroundService<T> : BackgroundService where T : IRecurringAction
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;
    private readonly Timing _timing;
    private readonly string _cron;

    public CronBackgroundService(IServiceScopeFactory scopeFactory, ILogger logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        using var scope = scopeFactory.CreateScope();
        var action = scope.ServiceProvider.GetRequiredService<T>();
        _cron = action.Cron;
        _timing = new Timing(action.GetTimeZoneId());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogTrace(
            $"Using {_cron} and timezone '{_timing.TimeZoneInfo.Id}. The time in this timezone: {_timing.RelativeNow()}'");
        DateTimeOffset? next = null;

        do
        {
            var now = _timing.RelativeNow();

            if (next == null)
            {
                next = _timing.GetNextOccurenceInRelativeTime(_cron);
                var uText = _timing.Get10NextOccurrences(_cron);
                var logText = $"Ten next occurrences :\n{uText.Aggregate((x, y) => x + "\n" + y)}";
                _logger.LogTrace(logText);
            }

            if (now > next)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var action = scope.ServiceProvider.GetRequiredService<T>();
                    await action.Process(stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }

                next = _timing.GetNextOccurenceInRelativeTime(_cron);
                _logger.LogTrace(next is not null
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
