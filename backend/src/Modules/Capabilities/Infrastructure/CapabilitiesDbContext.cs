using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Capabilities.Domain;

namespace SummitCms.Modules.Capabilities.Infrastructure;

public class CapabilitiesDbContext(DbContextOptions<CapabilitiesDbContext> options) : DbContext(options)
{
    public const string Schema = "capabilities";

    public DbSet<Capability> Capabilities => Set<Capability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Capability>(b =>
        {
            b.HasIndex(c => c.Key).IsUnique();
            b.Property(c => c.Key).HasMaxLength(64).IsRequired();
            b.Property(c => c.Name).HasMaxLength(200);
        });
    }
}
