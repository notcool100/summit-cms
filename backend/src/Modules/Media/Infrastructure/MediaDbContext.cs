using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Media.Domain;

namespace SummitCms.Modules.Media.Infrastructure;

public class MediaDbContext(DbContextOptions<MediaDbContext> options) : DbContext(options)
{
    public const string Schema = "media";

    public DbSet<MediaFolder> MediaFolders => Set<MediaFolder>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<MediaFolder>(b =>
        {
            b.Property(f => f.Name).HasMaxLength(200).IsRequired();
            b.HasOne(f => f.ParentFolder).WithMany().HasForeignKey(f => f.ParentFolderId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MediaAsset>(b =>
        {
            b.Property(a => a.FileName).HasMaxLength(300).IsRequired();
            b.Property(a => a.ContentType).HasMaxLength(150);
            b.Property(a => a.AltText).HasMaxLength(500);
            b.HasOne(a => a.Folder).WithMany().HasForeignKey(a => a.FolderId).OnDelete(DeleteBehavior.SetNull);
            b.ToTable(t => t.HasCheckConstraint(
                "ck_media_assets_source",
                "(source_type = 0 AND storage_key IS NOT NULL) OR (source_type = 1 AND external_url IS NOT NULL)"));
        });
    }
}
