using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notifications.Database;
using Notifications.Database.Models;
using System.Text;

namespace InboxEventsConsumer
{
    public class EventsListener : BackgroundService
    {
        private readonly NotificationsContext _dbContext;
        private readonly ILogger<EventsListener> _logger;
        private readonly IConsumer<string, string> _consumer;

        public EventsListener(NotificationsContext dbContext, IConsumer<string, string> consumer, ILogger<EventsListener> logger)
        {
            _dbContext = dbContext;
            _consumer = consumer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("users-events");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result == null)
                    {
                        continue;
                    }

                    var message = result.Message;
                    var eventId = Guid.Parse(message.Key);

                    var alreadyProcessed = await _dbContext.Events.AnyAsync(e => e.Id == eventId, stoppingToken);

                    if (alreadyProcessed)
                    {
                        _logger.LogInformation("Duplicate message skipped {EventId}", eventId);
                        _consumer.Commit(result);
                        continue;
                    }

                    var headers = message.Headers;
                    var eventType = headers.GetString("event-id");
                    var version = headers.GetInt("event-version");
                    var correlationId = headers.GetGuid("correlation-id");

                    _dbContext.Events.Add(new Event
                    {
                        Id = eventId,
                        CorrelationId = correlationId ?? Guid.Empty,
                        EventType = eventType,
                        Version = version,
                        Payload = message.Value
                    });

                    await _dbContext.SaveChangesAsync(stoppingToken);
                    
                    _consumer.Commit(result);

                    _logger.LogInformation("Message processed {EventId}", eventId);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error");
                }
            }

            _consumer.Close();
        }
    }

    public static class KafkaHeadersExtensions
    {
        public static string? GetString(this Headers headers, string key)
        {
            var h = headers.FirstOrDefault(x => x.Key == key);
            return h == null ? null : Encoding.UTF8.GetString(h.GetValueBytes());
        }

        public static Guid? GetGuid(this Headers headers, string key)
        {
            var h = headers.FirstOrDefault(x => x.Key == key);
            return h == null ? null : Guid.Parse(Encoding.UTF8.GetString(h.GetValueBytes()));
        }

        public static int GetInt(this Headers headers, string key)
        {
            var h = headers.FirstOrDefault(x => x.Key == key);
            return h == null ? 0 : BitConverter.ToInt32(h.GetValueBytes(), 0);
        }
    }
}