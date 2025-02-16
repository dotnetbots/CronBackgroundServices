using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CronBackgroundServices;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     For distributed apps
    /// </summary>
    public static IServiceCollection AddRecurrer<T>(this IServiceCollection services) where T : class, IRecurringAction
    {
        services.AddSingleton<IRecurringAction, T>();
        services.AddSingleton<IHostedService>(s =>
        {
            var allRecurrers = s.GetServices<IRecurringAction>();
            var single = allRecurrers.First(r => r is T);
            var loggerFactory = s.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<T>();
            return new CronBackgroundService(single, logger);
        });
        return services;
    }
}
