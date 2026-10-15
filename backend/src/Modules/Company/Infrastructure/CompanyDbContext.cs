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
    public DbSet<ContentSection> ContentSections => Set<ContentSection>();
    public DbSet<ContentItem> ContentItems => Set<ContentItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Milestone>(b => { b.Property(m => m.Year).HasMaxLength(16); b.Property(m => m.Title).HasMaxLength(200); });
        modelBuilder.Entity<CompanyValue>(b => { b.Property(v => v.Code).HasMaxLength(8); b.Property(v => v.Name).HasMaxLength(100); });
        modelBuilder.Entity<TeamMember>(b => { b.Property(t => t.Name).HasMaxLength(150); b.Property(t => t.Title).HasMaxLength(150); });
        modelBuilder.Entity<OfficeLocation>(b => b.Property(o => o.City).HasMaxLength(150));
        modelBuilder.Entity<Award>(b => { b.Property(a => a.Year).HasMaxLength(16); b.Property(a => a.Name).HasMaxLength(250); });
        modelBuilder.Entity<NarrativeBlock>(b => b.Property(n => n.Eyebrow).HasMaxLength(200));

        modelBuilder.Entity<ContentSection>(b =>
        {
            b.Property(c => c.Key).HasMaxLength(100);
            b.Property(c => c.PageSlug).HasMaxLength(50);
            b.Property(c => c.Slot).HasMaxLength(50);
            b.Property(c => c.Layout).HasMaxLength(20);
            b.Property(c => c.Eyebrow).HasMaxLength(100);
            b.Property(c => c.Heading).HasMaxLength(300);
            b.HasIndex(c => c.Key).IsUnique();
            b.HasIndex(c => c.PageSlug);
            b.HasMany(c => c.Items).WithOne().HasForeignKey(i => i.SectionId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ContentItem>(b =>
        {
            b.Property(i => i.Title).HasMaxLength(250);
            b.Property(i => i.Subtitle).HasMaxLength(150);
            b.Property(i => i.Detail).HasMaxLength(250);
            b.HasIndex(i => i.SectionId);
        });

        modelBuilder.Entity<Milestone>().HasIndex(m => m.PageId);
        modelBuilder.Entity<CompanyValue>().HasIndex(v => v.PageId);
        modelBuilder.Entity<TeamMember>().HasIndex(t => t.PageId);
        modelBuilder.Entity<OfficeLocation>().HasIndex(o => o.PageId);
        modelBuilder.Entity<Award>().HasIndex(a => a.PageId);
        modelBuilder.Entity<NarrativeBlock>().HasIndex(n => n.PageId);
    }
}
