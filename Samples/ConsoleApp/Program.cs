using Microsoft.Extensions.Hosting;
using CronBackgroundServices;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => services
        .AddRecurrer<EveryThreeSeconds>()
        .AddRecurrer<EveryFiveSeconds>())
    .Build()
    .Run();

public class EveryFiveSeconds : IRecurringAction
{
    public string Cron => "*/5 * * * * *";

    public Task Process(CancellationToken stoppingToken) => Logger.Log("🕔 Tick 5th second 5️⃣ 🖐");
}

public class EveryThreeSeconds : IRecurringAction
{
    public string Cron => "*/3 * * * * *";

    public Task Process(CancellationToken stoppingToken) => Logger.Log("🕒 Tick 3rd second 3️⃣ 🥉");
}

static class Logger
{
    public static Task Log(string msg)
    {
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }
}
