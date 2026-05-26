namespace Notifications.Database.Models;

public class Event
{
    public Guid Id { get; set; }

    public Guid CorrelationId { get; set; }

    public string EventType { get; set; }

    public int Version { get; set; }

    public string Payload { get; set; }

    public int RetryCount { get; set; }

    public string? LastError { get; set; }

    public DateTime? NextRetryAtUtc { get; set; }

    public DateTime? ProcessedAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
