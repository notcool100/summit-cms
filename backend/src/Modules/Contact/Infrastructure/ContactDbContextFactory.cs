using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SummitCms.Modules.Contact.Infrastructure;

public sealed class ContactDbContextFactory : IDesignTimeDbContextFactory<ContactDbContext>
{
    public ContactDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SUMMITCMS_CONNECTION")
            ?? throw new InvalidOperationException(
                "Set the SUMMITCMS_CONNECTION environment variable before running dotnet ef commands, " +
                "e.g. export SUMMITCMS_CONNECTION=\"Host=...;Port=...;Database=...;Username=...;Password=...\"");

        var builder = new DbContextOptionsBuilder<ContactDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", ContactDbContext.Schema))
            .UseSnakeCaseNamingConvention();

        return new ContactDbContext(builder.Options);
    }
}
