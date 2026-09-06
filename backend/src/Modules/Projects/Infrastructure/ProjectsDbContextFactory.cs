using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SummitCms.Modules.Projects.Infrastructure;

public sealed class ProjectsDbContextFactory : IDesignTimeDbContextFactory<ProjectsDbContext>
{
    public ProjectsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SUMMITCMS_CONNECTION")
            ?? throw new InvalidOperationException(
                "Set the SUMMITCMS_CONNECTION environment variable before running dotnet ef commands, " +
                "e.g. export SUMMITCMS_CONNECTION=\"Host=...;Port=...;Database=...;Username=...;Password=...\"");

        var builder = new DbContextOptionsBuilder<ProjectsDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", ProjectsDbContext.Schema))
            .UseSnakeCaseNamingConvention();

        return new ProjectsDbContext(builder.Options);
    }
}
