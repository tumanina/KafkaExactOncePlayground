using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OutboxEventsProcessor;
using Users.Database;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = "Host=localhost;Port=5432;Database=usersdatabase;Username=postgres;Password=playgroundpass";
builder.Services.AddDbContext<UsersContext>(options => options.UseNpgsql(connectionString));

RegisterProducer(builder);

builder.Services.AddHostedService<EventsProcessor>();

var host = builder.Build();

await host.RunAsync();

static void RegisterProducer(HostApplicationBuilder builder)
{
    builder.Services.AddSingleton<IProducer<string, string>>(_ =>
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            Acks = Acks.All,
            EnableIdempotence = true
        };

        return new ProducerBuilder<string, string>(config)
            .Build();
    });
}