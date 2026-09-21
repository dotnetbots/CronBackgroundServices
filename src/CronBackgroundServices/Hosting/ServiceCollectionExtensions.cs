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
        services.AddScoped<T>();
        services.AddSingleton<IHostedService>(s =>
        {
            var scopeFactory = s.GetRequiredService<IServiceScopeFactory>();
            var loggerFactory = s.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<T>();
            return new CronBackgroundService<T>(scopeFactory, logger);
        });
        return services;
    }
}
