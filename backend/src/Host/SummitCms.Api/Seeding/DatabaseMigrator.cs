using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Capabilities.Infrastructure;
using SummitCms.Modules.Careers.Infrastructure;
using SummitCms.Modules.Company.Infrastructure;
using SummitCms.Modules.Contact.Infrastructure;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Modules.Industries.Infrastructure;
using SummitCms.Modules.Media.Infrastructure;
using SummitCms.Modules.Projects.Infrastructure;
using SummitCms.Modules.SiteContent.Infrastructure;

namespace SummitCms.Api.Seeding;

/// <summary>Applies every module's pending migrations against its own schema on startup.</summary>
public static class DatabaseMigrator
{
    public static async Task MigrateAllAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        await sp.GetRequiredService<IdentityDbContext>().Database.MigrateAsync();
        await sp.GetRequiredService<MediaDbContext>().Database.MigrateAsync();
        await sp.GetRequiredService<SiteContentDbContext>().Database.MigrateAsync();
        await sp.GetRequiredService<CompanyDbContext>().Database.MigrateAsync();
        await sp.GetRequiredService<CapabilitiesDbContext>().Database.MigrateAsync();
        await sp.GetRequiredService<IndustriesDbContext>().Database.MigrateAsync();
        await sp.GetRequiredService<ProjectsDbContext>().Database.MigrateAsync();
        await sp.GetRequiredService<CareersDbContext>().Database.MigrateAsync();
        await sp.GetRequiredService<ContactDbContext>().Database.MigrateAsync();
    }
}
