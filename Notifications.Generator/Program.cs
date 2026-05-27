using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Database;
using Notifications.Generator;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = "Host=localhost;Port=5432;Database=notificationsdatabase;Username=postgres;Password=playgroundpass";
builder.Services.AddDbContext<NotificationsContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddHostedService<EventsProcessor>();

var host = builder.Build();

await host.RunAsync();
