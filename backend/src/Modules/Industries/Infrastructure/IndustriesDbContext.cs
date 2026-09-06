using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Industries.Domain;

namespace SummitCms.Modules.Industries.Infrastructure;

public class IndustriesDbContext(DbContextOptions<IndustriesDbContext> options) : DbContext(options)
{
    public const string Schema = "industries";

    public DbSet<Industry> Industries => Set<Industry>();
    public DbSet<IndustryProjectLink> ProjectLinks => Set<IndustryProjectLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Industry>(b => b.Property(i => i.Name).HasMaxLength(150).IsRequired());

        modelBuilder.Entity<IndustryProjectLink>(b =>
        {
            b.HasOne(l => l.Industry).WithMany(i => i.ProjectLinks).HasForeignKey(l => l.IndustryId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(l => l.IndustryId);
            // ProjectId is a soft reference into the Projects module's own schema - no DB-level FK across modules.
        });
    }
}
