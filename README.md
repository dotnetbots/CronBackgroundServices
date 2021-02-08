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
services.AddRecurringActions()
.AddRecurrer<MyCustomRecurringJob>()
.AddRecurrer<MySecondJob>()
.Build();
```

Each job has to implement `IRecurringAction`. If you want a different TimeZone than UTC you have to override the default interface method `GetTimeZoneId`.

```csharp
public interface IRecurringAction
{
    
    Task Process(CancellationToken stoppingToken);
    string Cron { get; }
    string GetTimeZoneId()
    {
        return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Europe/London" : "GMT Standard Time";
    }
}
```

