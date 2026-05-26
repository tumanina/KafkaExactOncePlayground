using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notifications.Database.Models;

namespace Users.Database.Configurations;

internal class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasIndex(x => x.CreatedAtUtc).HasFilter("\"ProcessedAtUtc\" IS NULL");
        builder.HasIndex(x => x.NextRetryAtUtc).HasFilter("\"ProcessedAtUtc\" IS NULL");
        builder.HasIndex(x => x.ProcessedAtUtc);
        builder.HasIndex(x => x.CorrelationId);
    }
}
