using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Projects.Domain;

namespace SummitCms.Modules.Projects.Infrastructure;

public class ProjectsDbContext(DbContextOptions<ProjectsDbContext> options) : DbContext(options)
{
    public const string Schema = "projects";

    public DbSet<ProjectIndustryCategory> IndustryCategories => Set<ProjectIndustryCategory>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectGalleryImage> GalleryImages => Set<ProjectGalleryImage>();
    public DbSet<ProjectScopeFact> ScopeFacts => Set<ProjectScopeFact>();
    public DbSet<ProjectNarrativeSection> NarrativeSections => Set<ProjectNarrativeSection>();
    public DbSet<ProjectNarrativeParagraph> NarrativeParagraphs => Set<ProjectNarrativeParagraph>();
    public DbSet<ProjectQuote> Quotes => Set<ProjectQuote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<ProjectIndustryCategory>(b =>
        {
            b.HasIndex(c => c.Name).IsUnique();
            b.Property(c => c.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Project>(b =>
        {
            b.HasIndex(p => p.Slug).IsUnique();
            b.Property(p => p.Slug).HasMaxLength(150).IsRequired();
            b.Property(p => p.Name).HasMaxLength(250).IsRequired();
            b.HasOne(p => p.IndustryCategory).WithMany().HasForeignKey(p => p.IndustryCategoryId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(p => p.Quote).WithOne(q => q.Project).HasForeignKey<ProjectQuote>(q => q.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectGalleryImage>(b =>
        {
            b.HasOne(g => g.Project).WithMany(p => p.GalleryImages).HasForeignKey(g => g.ProjectId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(g => g.ProjectId);
        });

        modelBuilder.Entity<ProjectScopeFact>(b =>
        {
            b.HasOne(f => f.Project).WithMany(p => p.ScopeFacts).HasForeignKey(f => f.ProjectId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(f => f.ProjectId);
        });

        modelBuilder.Entity<ProjectNarrativeSection>(b =>
        {
            b.HasOne(s => s.Project).WithMany(p => p.NarrativeSections).HasForeignKey(s => s.ProjectId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(s => s.ProjectId);
        });

        modelBuilder.Entity<ProjectNarrativeParagraph>(b =>
        {
            b.HasOne(p => p.NarrativeSection).WithMany(s => s.Paragraphs).HasForeignKey(p => p.NarrativeSectionId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(p => p.NarrativeSectionId);
        });
    }
}
