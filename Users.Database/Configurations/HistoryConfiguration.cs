using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Database.Models;

namespace Users.Database.Configurations;

internal class HistoryConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.Property(e => e.EventType).HasConversion<string>();
        builder.HasIndex(x => x.CreatedAtUtc).HasFilter("\"ProcessedAtUtc\" IS NULL");
        builder.HasIndex(x => x.NextRetryAtUtc).HasFilter("\"ProcessedAtUtc\" IS NULL");
        builder.HasIndex(x => x.ProcessedAtUtc);
        builder.HasIndex(x => x.CorrelationId);
    }
}
