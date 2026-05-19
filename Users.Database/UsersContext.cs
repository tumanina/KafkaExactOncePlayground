using Microsoft.EntityFrameworkCore;
using Users.Database.Models;

namespace Users.Database;

public class UsersContext : DbContext
{
    public UsersContext(DbContextOptions options) : base(options) { }
    public UsersContext() { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<User> Users { get; set; }
}
