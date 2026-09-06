using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SummitCms.Modules.Capabilities.Infrastructure;

public sealed class CapabilitiesDbContextFactory : IDesignTimeDbContextFactory<CapabilitiesDbContext>
{
    public CapabilitiesDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SUMMITCMS_CONNECTION")
            ?? throw new InvalidOperationException(
                "Set the SUMMITCMS_CONNECTION environment variable before running dotnet ef commands, " +
                "e.g. export SUMMITCMS_CONNECTION=\"Host=...;Port=...;Database=...;Username=...;Password=...\"");

        var builder = new DbContextOptionsBuilder<CapabilitiesDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", CapabilitiesDbContext.Schema))
            .UseSnakeCaseNamingConvention();

        return new CapabilitiesDbContext(builder.Options);
    }
}
