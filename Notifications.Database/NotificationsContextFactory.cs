using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Notifications.Database;

internal class NotificationsContextFactory : IDesignTimeDbContextFactory<NotificationsContext>
{
    public NotificationsContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotificationsContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=notificationsdatabase;Username=postgres;Password=playgroundpass");

        return new NotificationsContext(optionsBuilder.Options);
    }
}