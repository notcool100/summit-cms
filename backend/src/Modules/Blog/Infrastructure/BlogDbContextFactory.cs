using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SummitCms.Modules.Blog.Infrastructure;

public sealed class BlogDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
{
    public BlogDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SUMMITCMS_CONNECTION")
            ?? throw new InvalidOperationException(
                "Set the SUMMITCMS_CONNECTION environment variable before running dotnet ef commands, " +
                "e.g. export SUMMITCMS_CONNECTION=\"Host=...;Port=...;Database=...;Username=...;Password=...\"");

        var builder = new DbContextOptionsBuilder<BlogDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", BlogDbContext.Schema))
            .UseSnakeCaseNamingConvention();

        return new BlogDbContext(builder.Options);
    }
}
