using Microsoft.EntityFrameworkCore;
using Notifications.Database;

var connectionString = "Host=localhost;Port=5432;Database=notificationsdatabase;Username=postgres;Password=playgroundpass";
var builder = new DbContextOptionsBuilder<DbContext>();

builder.EnableSensitiveDataLogging()
       .UseNpgsql(connectionString,
                     opts =>
                     {
                         opts.CommandTimeout((int)TimeSpan.FromMinutes(120).TotalSeconds);
                         opts.EnableRetryOnFailure();
                     });

var dbContext = new NotificationsContext(builder.Options);
dbContext.Database.Migrate();