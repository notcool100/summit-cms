using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SummitCms.Shared.Infrastructure.Persistence;

public static class ModuleDbContextExtensions
{
    /// <summary>
    /// Registers a module's own <see cref="DbContext"/> against the shared Postgres database, scoped to
    /// its own schema (default schema + its own migrations-history table), with snake_case naming and
    /// the CreatedAt/UpdatedAt stamping interceptor applied. Every module calls this once for its context.
    /// </summary>
    public static IServiceCollection AddModuleDbContext<TContext>(
        this IServiceCollection services, IConfiguration configuration, string schema)
        where TContext : DbContext
    {
        services.AddSingleton<AuditableEntitySaveChangesInterceptor>();
        services.AddDbContext<TContext>((sp, options) =>
        {
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Default"),
                    npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", schema))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
        });

        return services;
    }
}
