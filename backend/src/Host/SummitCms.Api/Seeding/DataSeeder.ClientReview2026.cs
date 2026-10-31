using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Blog.Infrastructure;
using SummitCms.Modules.Company.Infrastructure;
using SummitCms.Modules.Media.Infrastructure;
using SummitCms.Modules.Projects.Domain;
using SummitCms.Modules.Projects.Infrastructure;
using SummitCms.Modules.SiteContent.Domain;
using SummitCms.Modules.SiteContent.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Api.Seeding;

/// <summary>
/// Applies the October 2026 client website review ("Summit_Website_Review_With_Visuals_2026.pdf") to a
/// database seeded before it. Two parts:
/// <list type="bullet">
/// <item>Image provenance (P0-01) runs on every start: any reference to a summit.us image is swapped for
/// an illustrative stock image and the summit.us media rows are deleted, so they can never come back.</item>
/// <item>Copy changes (P0-02, P0-03, P1-07, P2-01, P2-02) run once, guarded by a site setting marker, so
/// later edits made in the admin panel are not overwritten on the next restart.</item>
/// </list>
/// </summary>
public static partial class DataSeeder
{
    private const string ReviewMarkerKey = "content_revision";
    private const string ReviewMarkerValue = "client-review-2026-10";

    private static async Task ApplyClientReview2026Async(IServiceProvider sp, Dictionary<string, Guid> mediaMap)
    {
        Guid Media(string slug) => mediaMap[Stock(slug)];

        await ReplaceThirdPartyImagesAsync(sp, Media);
        await ApplyReviewedCopyOnceAsync(sp);
        await RetireThirdPartyMediaAsync(sp);
    }

    private static async Task ReplaceThirdPartyImagesAsync(IServiceProvider sp, Func<string, Guid> media)
    {
        if (ThirdPartyMediaIds.Count == 0) return;

        // About page narrative blocks: the first used a summit.us yard photo captioned as 1996.
        var company = sp.GetRequiredService<CompanyDbContext>();
        foreach (var block in await company.NarrativeBlocks.ToListAsync())
        {
            if (!IsThirdPartyMedia(block.MediaId)) continue;
            block.MediaId = block.Eyebrow == "Our Approach" ? media("road-train-logistics") : media("site-planning-engineers");
            block.ImageCaption = "ILLUSTRATIVE IMAGE";
        }
        // Portraits from summit.us were of a different company's executives - never show them.
        foreach (var member in await company.TeamMembers.Where(m => m.MediaId != null).ToListAsync())
            if (IsThirdPartyMedia(member.MediaId)) member.MediaId = null;
        await company.SaveChangesAsync();

        // Work scope gallery images follow the scope's (already replaced) hero image.
        var projects = sp.GetRequiredService<ProjectsDbContext>();
        var gallery = await projects.GalleryImages.ToListAsync();
        var heroes = await projects.Projects.ToDictionaryAsync(p => p.Id, p => p.HeroMediaId);
        foreach (var image in gallery.Where(g => IsThirdPartyMedia(g.MediaId)))
        {
            if (heroes.TryGetValue(image.ProjectId, out var hero) && hero is Guid heroId && !IsThirdPartyMedia(heroId))
                image.MediaId = heroId;
            else
                projects.GalleryImages.Remove(image);
        }
        foreach (var project in await projects.Projects.Where(p => p.HeroMediaId != null).ToListAsync())
            if (IsThirdPartyMedia(project.HeroMediaId)) project.HeroMediaId = media("structural-access-frames");
        await projects.SaveChangesAsync();

        // Insight covers, matched to each article's topic.
        var blog = sp.GetRequiredService<BlogDbContext>();
        foreach (var post in await blog.Posts.Where(p => p.CoverMediaId != null).ToListAsync())
        {
            if (!IsThirdPartyMedia(post.CoverMediaId)) continue;
            post.CoverMediaId = post.Category switch
            {
                "Safety" => media("work-at-height-harness"),
                "Project Planning" => media("site-planning-engineers"),
                "Rigging" => media("mobile-crane-outriggers"),
                "Logistics" => media("road-train-logistics"),
                _ => post.Slug.Contains("system", StringComparison.OrdinalIgnoreCase)
                    ? media("scaffold-tower-crew")
                    : media("scaffold-crew-deck")
            };
        }
        await blog.SaveChangesAsync();

        // Page version history must not keep the old images published or restorable.
        var site = sp.GetRequiredService<SiteContentDbContext>();
        foreach (var version in await site.PageVersions.ToListAsync())
        {
            if (IsThirdPartyMedia(version.HeroMediaId)) version.HeroMediaId = null;
            if (IsThirdPartyMedia(version.SecondaryMediaId)) version.SecondaryMediaId = null;
        }
        foreach (var page in await site.Pages.ToListAsync())
        {
            if (IsThirdPartyMedia(page.HeroMediaId)) page.HeroMediaId = page.Slug == PageSlugs.Home ? media("scaffold-crew-deck") : null;
            if (IsThirdPartyMedia(page.SecondaryMediaId)) page.SecondaryMediaId = page.Slug == PageSlugs.Home ? media("scaffold-tower-crew") : null;
        }
        await site.SaveChangesAsync();
    }

    private static async Task RetireThirdPartyMediaAsync(IServiceProvider sp)
    {
        if (ThirdPartyMediaIds.Count == 0) return;

        var db = sp.GetRequiredService<MediaDbContext>();
        var rows = await db.MediaAssets.Where(a => ThirdPartyMediaIds.Contains(a.Id)).ToListAsync();
        db.MediaAssets.RemoveRange(rows);
        await db.SaveChangesAsync();
        ThirdPartyMediaIds.Clear();
    }

    private static async Task ApplyReviewedCopyOnceAsync(IServiceProvider sp)
    {
        var site = sp.GetRequiredService<SiteContentDbContext>();
        var marker = await site.SiteSettings.FirstOrDefaultAsync(s => s.Key == ReviewMarkerKey);
        if (marker?.Value == ReviewMarkerValue) return;

        // Pages: titles, descriptions and hero copy, published as a new version so history shows the change.
        var pages = await site.Pages.ToListAsync();
        foreach (var (slug, title, meta, hero, sub) in ReviewedPageCopy)
        {
            var page = pages.FirstOrDefault(p => p.Slug == slug);
            if (page is null) continue;
            if (page.Title == title && page.MetaDescription == meta && page.HeroHeading == hero && page.HeroSubheading == sub)
                continue;

            var previouslyPublished = await site.PageVersions.Where(v => v.PageId == page.Id && v.IsPublished).ToListAsync();
            foreach (var v in previouslyPublished) v.IsPublished = false;
            var nextNumber = (await site.PageVersions.Where(v => v.PageId == page.Id).MaxAsync(v => (int?)v.VersionNumber) ?? 0) + 1;

            var version = new PageVersion
            {
                PageId = page.Id,
                VersionNumber = nextNumber,
                Title = title,
                MetaDescription = meta,
                HeroHeading = hero,
                HeroSubheading = sub,
                HeroMediaId = page.HeroMediaId,
                SecondaryMediaId = page.SecondaryMediaId,
                IsPublished = true,
                CreatedByUserId = null,
                CreatedAt = DateTimeOffset.UtcNow
            };
            site.PageVersions.Add(version);

            page.Title = title;
            page.MetaDescription = meta;
            page.HeroHeading = hero;
            page.HeroSubheading = sub;
            page.PublishedVersionId = version.Id;
            page.PublishedAt = version.CreatedAt;
        }

        // Company / About page: no invented history, captions must not imply a real Summit location.
        var company = sp.GetRequiredService<CompanyDbContext>();
        foreach (var block in await company.NarrativeBlocks.ToListAsync())
            if (block.ImageCaption is "KARRATHA INDUSTRIAL ESTATE" or "COORDINATED EXECUTION")
                block.ImageCaption = "ILLUSTRATIVE IMAGE";

        var sections = await company.ContentSections.ToListAsync();
        void SetSection(string key, Action<SummitCms.Modules.Company.Domain.ContentSection> change)
        {
            var section = sections.FirstOrDefault(s => s.Key == key);
            if (section is not null) change(section);
        }
        // The safety slogan already appears in the home marquee directly below - don't repeat it.
        SetSection("home-manifesto", s => { if (s.Note == "Every person goes home safely, every day.") s.Note = ""; });
        // Keep the engineering qualification visible in the short headline too (P1-08).
        SetSection("capabilities-engineering", s => s.Heading = "Engineering coordination,\nwhere required.");
        // Articles are general information, not field reports (P1-07).
        SetSection("insights-overview", s => s.Heading = "Capability notes,\nby topic.");
        await company.SaveChangesAsync();

        var items = await company.ContentItems.ToListAsync();
        foreach (var item in items)
        {
            if (item.Title == "Engineer verification" && item.Body == "Verification by suitably qualified engineers.")
                item.Body = "Verification by suitably qualified engineers where engineered design is required.";
            if (item.Title == "How can I contact Summit?")
                item.Body = "Send an enquiry using the form on this page, call 0401 174 989, email support@summit-is.com.au for general enquiries, or contact Jason Matakatea directly at Jason@summit-is.com.au.";
        }
        await company.SaveChangesAsync();

        // Work scopes: the "Profile type" fact becomes an unmistakable status line (P0-03).
        var projects = sp.GetRequiredService<ProjectsDbContext>();
        foreach (var fact in await projects.ScopeFacts.Where(f => f.Label == "Profile type").ToListAsync())
        {
            fact.Label = WorkScopeFactLabel;
            fact.Value = WorkScopeFactValue;
        }
        foreach (var image in await projects.GalleryImages.Where(g => g.Role == ProjectImageRole.Break).ToListAsync())
            if (!image.Caption.StartsWith("ILLUSTRATIVE", StringComparison.Ordinal))
                image.Caption = $"ILLUSTRATIVE IMAGE · {image.Caption}";
        await projects.SaveChangesAsync();

        if (marker is null)
            site.SiteSettings.Add(new SiteSetting { Key = ReviewMarkerKey, Value = ReviewMarkerValue, ValueType = "string" });
        else
            marker.Value = ReviewMarkerValue;
        await site.SaveChangesAsync();
    }
}
