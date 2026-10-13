using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Blog.Domain;
using SummitCms.Modules.Blog.Infrastructure;
using SummitCms.Modules.Industries.Infrastructure;
using SummitCms.Modules.Projects.Infrastructure;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task<Dictionary<string, Guid>> RetireUnverifiedProjectSamplesAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<ProjectsDbContext>();
        string[] sampleSlugs =
        [
            "phoenix",
            "processing-plant-foundation-trestle",
            "osm-yard-modular-assembly",
            "project-star-modular-trestles",
            "newman-plant-expansion",
            "pilbara-marine-terminal",
            "ironbark-power-station",
            "ridgeline-biomass-facility"
        ];

        var samples = await db.Projects.Where(p => sampleSlugs.Contains(p.Slug)).ToListAsync();
        var sampleIds = samples.Select(p => p.Id).ToArray();
        if (sampleIds.Length > 0)
        {
            var industriesDb = sp.GetRequiredService<IndustriesDbContext>();
            var links = await industriesDb.ProjectLinks.Where(link => sampleIds.Contains(link.ProjectId)).ToListAsync();
            industriesDb.ProjectLinks.RemoveRange(links);
            await industriesDb.SaveChangesAsync();
        }

        db.Projects.RemoveRange(samples);
        await db.SaveChangesAsync();

        return new Dictionary<string, Guid>();
    }

    private static async Task RetireUnverifiedBlogSamplesAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<BlogDbContext>();
        string[] sampleSlugs =
        [
            "what-18-million-safe-hours-requires",
            "why-summit-still-direct-hires",
            "inside-the-osm-yard",
            "pilbara-marine-terminal-mechanical-completion",
            "minerals-processing-buildout-mechanical-contracting",
            "winning-the-worksafe-wa-award"
        ];

        var samples = await db.Posts.Where(p => sampleSlugs.Contains(p.Slug)).ToListAsync();
        foreach (var post in samples)
        {
            post.Status = BlogPostStatus.Draft;
            post.IsFeatured = false;
        }

        await db.SaveChangesAsync();
    }
}