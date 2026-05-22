using Microsoft.EntityFrameworkCore;
using Users.Database;

var connectionString = "Host=localhost;Port=5432;Database=usersdatabase;Username=postgres;Password=playgroundpass";
var builder = new DbContextOptionsBuilder<DbContext>();

builder.EnableSensitiveDataLogging()
       .UseNpgsql(connectionString,
                     opts =>
                     {
                         opts.CommandTimeout((int)TimeSpan.FromMinutes(120).TotalSeconds);
                         opts.EnableRetryOnFailure();
                     });

var dbContext = new UsersContext(builder.Options);
dbContext.Database.Migrate();
