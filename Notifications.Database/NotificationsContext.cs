using Microsoft.EntityFrameworkCore;
using Notifications.Database.Models;

namespace Notifications.Database;

public class NotificationsContext : DbContext
{
    public NotificationsContext(DbContextOptions options) : base(options) { }
    public NotificationsContext() { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Event> Events { get; set; }
}
