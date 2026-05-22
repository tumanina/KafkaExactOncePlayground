using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Users.Database;

internal class UsersContextFactory : IDesignTimeDbContextFactory<UsersContext>
{
    public UsersContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UsersContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=usersdatabase;Username=postgres;Password=playgroundpass");

        return new UsersContext(optionsBuilder.Options);
    }
}