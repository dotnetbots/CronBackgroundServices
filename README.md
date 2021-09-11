[![main](https://github.com/slackbot-net/CronBackgroundServices/workflows/CI/badge.svg)](https://github.com/slackbot-net/CronBackgroundServices/actions) [![NuGet](https://img.shields.io/nuget/v/CronBackgroundServices.svg)](https://www.nuget.org/packages/CronBackgroundServices/)
[![NuGet](https://img.shields.io/nuget/vpre/CronBackgroundServices.svg)](https://www.nuget.org/packages/CronBackgroundServices/)

### CronBackgroundServices

.NET BackgroundService jobs triggered by configured Cron Expressions

### Installation

```bash
$ dotnet add package CronBackgroundServices
```

### Usage
Jobs are configured during DI registration:

```csharp
services
.AddRecurrer<MyCustomRecurringJob>()
.AddRecurrer<MySecondJob>()
```

Each job has to implement `IRecurringAction`. If you want a different TimeZone than UTC you have to override the default interface method `GetTimeZoneId`.

```csharp
public interface IRecurringAction
{

     /// <summary>
    /// The job to be executed at intervals defined by the Cron expression
    /// </summary>
    /// <returns></returns>
    Task Process(CancellationToken stoppingToken);

    /// <summary>
    /// The cron expression (including seconds) as defined by the Cronos library:
    /// See https://github.com/HangfireIO/Cronos#cron-format
    /// Ex: Every second: */1 * * * * *
    /// Ex: Every minute: 0 */1 * * * *
    /// Ex: Every midnight: 0 0 */1 * * *
    /// Ex: First of every month 0 0 0 1 * *
    /// </summary>
    /// <returns>A valid Cron Expression</returns>
    string Cron { get; }

    /// <summary>
    /// Optional: The TimeZone in which the Cron expression should be based on.
    /// Defaults to UTC (Europe/London or GMT Standard Time)
    ///
    /// NB! When overriding this and targeting versions below .NET 6, use platform specific identifiers
    /// If your runtime is .NET 6 or above, it's not required. It will handles the conversion:
    /// See https://github.com/dotnet/runtime/pull/49412
    /// </summary>
    /// <returns>timezoneId</returns>
    string GetTimeZoneId()
    {
        return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Europe/London" : "GMT Standard Time";
    }
}
```

