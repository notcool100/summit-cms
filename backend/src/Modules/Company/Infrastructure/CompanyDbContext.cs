using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Company.Domain;

namespace SummitCms.Modules.Company.Infrastructure;

public class CompanyDbContext(DbContextOptions<CompanyDbContext> options) : DbContext(options)
{
    public const string Schema = "company";

    public DbSet<Milestone> Milestones => Set<Milestone>();
    public DbSet<CompanyValue> CompanyValues => Set<CompanyValue>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<OfficeLocation> OfficeLocations => Set<OfficeLocation>();
    public DbSet<Award> Awards => Set<Award>();
    public DbSet<NarrativeBlock> NarrativeBlocks => Set<NarrativeBlock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Milestone>(b => { b.Property(m => m.Year).HasMaxLength(16); b.Property(m => m.Title).HasMaxLength(200); });
        modelBuilder.Entity<CompanyValue>(b => { b.Property(v => v.Code).HasMaxLength(8); b.Property(v => v.Name).HasMaxLength(100); });
        modelBuilder.Entity<TeamMember>(b => { b.Property(t => t.Name).HasMaxLength(150); b.Property(t => t.Title).HasMaxLength(150); });
        modelBuilder.Entity<OfficeLocation>(b => b.Property(o => o.City).HasMaxLength(150));
        modelBuilder.Entity<Award>(b => { b.Property(a => a.Year).HasMaxLength(16); b.Property(a => a.Name).HasMaxLength(250); });
        modelBuilder.Entity<NarrativeBlock>(b => b.Property(n => n.Eyebrow).HasMaxLength(200));

        modelBuilder.Entity<Milestone>().HasIndex(m => m.PageId);
        modelBuilder.Entity<CompanyValue>().HasIndex(v => v.PageId);
        modelBuilder.Entity<TeamMember>().HasIndex(t => t.PageId);
        modelBuilder.Entity<OfficeLocation>().HasIndex(o => o.PageId);
        modelBuilder.Entity<Award>().HasIndex(a => a.PageId);
        modelBuilder.Entity<NarrativeBlock>().HasIndex(n => n.PageId);
    }
}
