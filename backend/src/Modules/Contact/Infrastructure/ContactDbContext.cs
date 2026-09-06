using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Contact.Domain;

namespace SummitCms.Modules.Contact.Infrastructure;

public class ContactDbContext(DbContextOptions<ContactDbContext> options) : DbContext(options)
{
    public const string Schema = "contact";

    public DbSet<ContactSubmission> Submissions => Set<ContactSubmission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<ContactSubmission>(b =>
        {
            b.Property(s => s.Name).HasMaxLength(200).IsRequired();
            b.Property(s => s.Email).HasMaxLength(256).IsRequired();
            b.HasIndex(s => s.Status);
            b.HasIndex(s => s.CreatedAt);
        });
    }
}
