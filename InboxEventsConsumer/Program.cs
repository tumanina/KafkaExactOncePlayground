using Confluent.Kafka;
using InboxEventsConsumer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notifications.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(x =>
{
    x.AddConsole();
});

RegisterConsumer(builder);

builder.Services.AddDbContext<NotificationsContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=notificationsdatabase;Username=postgres;Password=playgroundpass");
});

builder.Services.AddHostedService<EventsListener>();

await builder.Build().RunAsync();

static void RegisterConsumer(HostApplicationBuilder builder)
{
    builder.Services.AddSingleton<IConsumer<string, string>>(_ =>
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "notifications-service",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        return new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => { Console.WriteLine($"Kafka error: {e.Reason}"); })
            .Build();
    });
}