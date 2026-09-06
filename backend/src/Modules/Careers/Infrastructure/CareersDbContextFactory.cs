using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SummitCms.Modules.Careers.Infrastructure;

public sealed class CareersDbContextFactory : IDesignTimeDbContextFactory<CareersDbContext>
{
    public CareersDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SUMMITCMS_CONNECTION")
            ?? throw new InvalidOperationException(
                "Set the SUMMITCMS_CONNECTION environment variable before running dotnet ef commands, " +
                "e.g. export SUMMITCMS_CONNECTION=\"Host=...;Port=...;Database=...;Username=...;Password=...\"");

        var builder = new DbContextOptionsBuilder<CareersDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", CareersDbContext.Schema))
            .UseSnakeCaseNamingConvention();

        return new CareersDbContext(builder.Options);
    }
}
