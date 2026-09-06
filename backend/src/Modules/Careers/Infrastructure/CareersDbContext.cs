using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Careers.Domain;

namespace SummitCms.Modules.Careers.Infrastructure;

public class CareersDbContext(DbContextOptions<CareersDbContext> options) : DbContext(options)
{
    public const string Schema = "careers";

    public DbSet<JobTrack> JobTracks => Set<JobTrack>();
    public DbSet<JobTrackTag> JobTrackTags => Set<JobTrackTag>();
    public DbSet<JobOpening> JobOpenings => Set<JobOpening>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<JobTrack>(b =>
        {
            b.Property(t => t.Title).HasMaxLength(150);
            b.HasIndex(t => t.PageId);
        });

        modelBuilder.Entity<JobTrackTag>(b =>
        {
            b.HasOne(t => t.JobTrack).WithMany(j => j.Tags).HasForeignKey(t => t.JobTrackId).OnDelete(DeleteBehavior.Cascade);
            b.Property(t => t.Tag).HasMaxLength(100);
        });

        modelBuilder.Entity<JobOpening>(b =>
        {
            b.Property(j => j.Title).HasMaxLength(200).IsRequired();
            b.Property(j => j.Department).HasMaxLength(150);
            b.HasIndex(j => j.IsActive);
        });
    }
}
