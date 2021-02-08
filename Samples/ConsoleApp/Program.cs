using System;
using System.Threading;
using System.Threading.Tasks;
using CronBackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
        
Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Trace))
    .ConfigureServices((_, services) => services.AddRecurringActions().AddRecurrer<MyCustomRecurringJob>().Build())
    .Build()
    .Run();

public class MyCustomRecurringJob : IRecurringAction
{
    public Task Process(CancellationToken stoppingToken)
    {
        Console.WriteLine("Tick");
        return Task.CompletedTask;
    }

    public string Cron => "*/30 0 */1 * * *"; // Every 30 seconds, in the zero-th minute, every hour, https://github.com/HangfireIO/Cronos#usage
}

