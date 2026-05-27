using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notifications.Database;

namespace Notifications.Generator
{
    public class EventsProcessor : BackgroundService
    {
        private const int BatchSize = 10;

        private readonly NotificationsContext _dbContext;
        private readonly ILogger<EventsProcessor> _logger;

        public EventsProcessor(NotificationsContext dbContext, ILogger<EventsProcessor> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification generator started");

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
                            if (eventToProcess.EventType == "user_created")
                            {
                                _logger.LogInformation("Welcome notification generation");
                            }

                            eventToProcess.ProcessedAtUtc = DateTime.UtcNow;
                            eventToProcess.LastError = null;
                        }
                        catch (Exception ex)
                        {
                            eventToProcess.RetryCount++;
                            eventToProcess.LastError = ex.ToString();
                            eventToProcess.NextRetryAtUtc = DateTime.UtcNow.AddSeconds(Math.Pow(2, eventToProcess.RetryCount));

                            _logger.LogError(ex, "Failed to process event {EventId}", eventToProcess.Id);
                        }
                    }

                    await _dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Critical notifications worker error");

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("Notifications generator stopped");
        }
    }
}