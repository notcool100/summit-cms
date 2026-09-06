using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.SiteContent.Domain;

namespace SummitCms.Modules.SiteContent.Infrastructure;

public class SiteContentDbContext(DbContextOptions<SiteContentDbContext> options) : DbContext(options)
{
    public const string Schema = "content";

    public DbSet<Page> Pages => Set<Page>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<EnquiryType> EnquiryTypes => Set<EnquiryType>();
    public DbSet<MetricStat> MetricStats => Set<MetricStat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Page>(b =>
        {
            b.HasIndex(p => p.Slug).IsUnique();
            b.Property(p => p.Slug).HasMaxLength(64).IsRequired();
        });

        modelBuilder.Entity<SiteSetting>(b =>
        {
            b.HasIndex(s => s.Key).IsUnique();
            b.Property(s => s.Key).HasMaxLength(128).IsRequired();
        });

        modelBuilder.Entity<EnquiryType>(b => b.Property(e => e.Label).HasMaxLength(200).IsRequired());

        modelBuilder.Entity<MetricStat>(b =>
        {
            b.HasOne(m => m.Page).WithMany().HasForeignKey(m => m.PageId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(m => new { m.PageId, m.GroupKey });
            b.Property(m => m.Value).HasPrecision(18, 2);
        });
    }
}
