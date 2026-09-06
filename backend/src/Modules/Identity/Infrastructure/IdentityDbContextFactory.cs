using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SummitCms.Modules.Identity.Infrastructure;

/// <summary>Used only by `dotnet ef migrations` at design time - the running app wires this up via AddModuleDbContext.</summary>
public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SUMMITCMS_CONNECTION")
            ?? throw new InvalidOperationException(
                "Set the SUMMITCMS_CONNECTION environment variable before running dotnet ef commands, " +
                "e.g. export SUMMITCMS_CONNECTION=\"Host=...;Port=...;Database=...;Username=...;Password=...\"");

        var builder = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", IdentityDbContext.Schema))
            .UseSnakeCaseNamingConvention();

        return new IdentityDbContext(builder.Options);
    }
}
