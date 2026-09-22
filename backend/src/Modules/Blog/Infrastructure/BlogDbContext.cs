using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Blog.Domain;

namespace SummitCms.Modules.Blog.Infrastructure;

public class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
    public const string Schema = "blog";

    public DbSet<BlogPost> Posts => Set<BlogPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<BlogPost>(b =>
        {
            b.HasIndex(p => p.Slug).IsUnique();
            b.Property(p => p.Slug).HasMaxLength(150).IsRequired();
            b.Property(p => p.Title).HasMaxLength(250).IsRequired();
            b.Property(p => p.Excerpt).HasMaxLength(500).IsRequired();
            b.Property(p => p.Category).HasMaxLength(100).IsRequired();
            b.Property(p => p.AuthorName).HasMaxLength(150).IsRequired();
            b.Property(p => p.AuthorRole).HasMaxLength(150).IsRequired();
            b.HasIndex(p => new { p.Status, p.PublishedAt });
        });
    }
}
