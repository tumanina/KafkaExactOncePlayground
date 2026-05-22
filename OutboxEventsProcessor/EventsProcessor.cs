using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using Users.Database;

namespace OutboxEventsProcessor
{
    public class EventsProcessor : BackgroundService
    {
        private const int BatchSize = 10;

        private readonly UsersContext _dbContext;
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<EventsProcessor> _logger;

        public EventsProcessor(UsersContext dbContext, IProducer<string, string> producer, ILogger<EventsProcessor> logger)
        {
            _dbContext = dbContext;
            _producer = producer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox processor started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;

                    var events = await _dbContext.Events
                        .Where(x => x.ProcessedAtUtc == null && (x.NextRetryAtUtc == null || x.NextRetryAtUtc <= now))
                        .OrderBy(x => x.CreatedAtUtc)
                        .Take(BatchSize)
                        .ToListAsync(stoppingToken);

                    if (events.Count == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                        continue;
                    }

                    foreach (var eventToProcess in events)
                    {
                        try
                        {
                            var message = new Message<string, string>
                            {
                                Key = eventToProcess.Id.ToString(),
                                Value = eventToProcess.Payload,
                                Headers = new Headers
                                {
                                    { "event-id", Encoding.UTF8.GetBytes(eventToProcess.Id.ToString()) },
                                    { "correlation-id", Encoding.UTF8.GetBytes(eventToProcess.CorrelationId.ToString())},
                                    { "event-type", Encoding.UTF8.GetBytes(eventToProcess.EventType.ToString()) },
                                    { "event-version", BitConverter.GetBytes(eventToProcess.Version) }
                                }
                            };

                            var result = await _producer.ProduceAsync(topic: "users-events", message, cancellationToken: stoppingToken);

                            eventToProcess.ProcessedAtUtc = DateTime.UtcNow;
                            eventToProcess.LastError = null;

                            _logger.LogInformation("Event {EventId} published to Kafka partition {Partition} offset {Offset}",
                                eventToProcess.Id,  result.Partition, result.Offset);
                        }
                        catch (Exception ex)
                        {
                            eventToProcess.RetryCount++;
                            eventToProcess.LastError = ex.ToString();
                            eventToProcess.NextRetryAtUtc = DateTime.UtcNow.AddSeconds(Math.Pow(2, eventToProcess.RetryCount));

                            _logger.LogError(ex, "Failed to publish event {EventId}", eventToProcess.Id);
                        }
                    }

                    await _dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Critical outbox worker error");

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("Outbox worker stopped");
        }
    }
}