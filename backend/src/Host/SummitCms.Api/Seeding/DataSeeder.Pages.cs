using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.SiteContent.Domain;
using SummitCms.Modules.SiteContent.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task<Dictionary<string, Guid>> SeedPagesAsync(IServiceProvider sp, Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<SiteContentDbContext>();

        (string Slug, string Title, string Meta, string Hero, string Sub)[] pages =
        [
            (PageSlugs.Home, "Summit Industrial Services | Industrial Services", "Self-performed mechanical, structural, and modular construction for the Pilbara's most demanding industrial sites.", "Built by the people who show up.", "2,400+ direct-hire craft. 18M+ safe work hours. Self-perform or don't bid."),
            (PageSlugs.About, "About Summit Industrial Services", "Founded in 1996 on a piping contract nobody else would touch. Now 2,400 craft strong.", "Self-perform or don't bid.", "The story, the values, and the people behind Summit Industrial Services."),
            (PageSlugs.Capabilities, "Capabilities", "Mechanical, structural, modular, underground, equipment setting, and design-assist engineering.", "Every discipline the work needs, under one roof.", "From chrome piping to constructability engineering."),
            (PageSlugs.Contact, "Contact Summit Industrial Services", "Get in touch about a project, a career, or a partnership.", "Let's talk about the work.", "New projects, design-assist, employment, or vendor inquiries."),
            (PageSlugs.Industries, "Industries We Serve", "Mining & minerals processing, power, energy & terminals, renewables, and heavy manufacturing.", "Industries that can't afford to slip.", "Schedule-critical construction across five industries."),
            (PageSlugs.Projects, "Featured Projects", "480,000 LF of pipe. 900,000 BBL of storage. 7 recommissioned power units.", "The scale of work we self-perform.", "A sample of what Summit crews have delivered."),
            (PageSlugs.Insights, "Insights", "Notes on safety, craft workforce, modular construction, and the industries Summit builds for.", "Notes from the field.", "What our crews, engineers, and leadership are seeing on the ground.")
        ];

        var existingPages = await db.Pages.ToDictionaryAsync(p => p.Slug, p => p);

        foreach (var (slug, title, meta, hero, sub) in pages)
        {
            if (existingPages.ContainsKey(slug)) continue;

            var page = new Page { Slug = slug, Title = title, MetaDescription = meta, HeroHeading = hero, HeroSubheading = sub };
            db.Pages.Add(page);
            existingPages[slug] = page;
        }

        // Backfill hero/secondary media for the home page, which originally had page-level hero art -
        // idempotent (only fills nulls) so it heals a database seeded before this field existed.
        Guid Media(string url) => mediaMap[url];

        if (existingPages.TryGetValue(PageSlugs.Home, out var homePage))
        {
            homePage.HeroMediaId ??= Media("https://summit.us/wp-content/uploads/2022/05/IMG_5350-scaled.jpg");
            homePage.SecondaryMediaId ??= Media("https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg");
        }

        await db.SaveChangesAsync();

        return existingPages.ToDictionary(kv => kv.Key, kv => kv.Value.Id);
    }

    private static async Task SeedSiteSettingsAndEnquiryTypesAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<SiteContentDbContext>();

        (string Key, string Value, string Type)[] settings =
        [
            ("company_name", "Summit Industrial Services", "string"),
            ("company_phone", "0401 174 989", "string"),
            ("company_email", "info@summit-is.com.au", "email"),
            ("company_address", "1537 Pyramid Road, Karratha Industrial Estate, WA 6714", "string"),
            ("footer_text", $"© {DateTimeOffset.UtcNow.Year} Summit Industrial Services. All rights reserved.", "string")
        ];
        var existingKeys = await db.SiteSettings.Select(s => s.Key).ToListAsync();
        foreach (var (key, value, type) in settings)
        {
            if (!existingKeys.Contains(key))
                db.SiteSettings.Add(new SiteSetting { Key = key, Value = value, ValueType = type });
        }

        if (!await db.EnquiryTypes.AnyAsync())
        {
            string[] labels = ["New project / RFP", "Design-assist engagement", "Craft employment", "Professional employment", "Vendor / subcontractor", "Media"];
            for (var i = 0; i < labels.Length; i++)
                db.EnquiryTypes.Add(new EnquiryType { Label = labels[i], DisplayOrder = i, IsActive = true });
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Backfills a version-1 PageVersion for any Page that predates page-versioning (or was inserted
    /// this run) and has no versions yet, then marks it published. Idempotent - only touches pages with
    /// zero PageVersion rows, so it's safe to run on every startup alongside SeedPagesAsync.
    /// </summary>
    private static async Task SeedPageVersionsAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<SiteContentDbContext>();

        var pagesWithoutVersions = await db.Pages
            .Where(p => !db.PageVersions.Any(v => v.PageId == p.Id))
            .ToListAsync();

        foreach (var page in pagesWithoutVersions)
        {
            var version = new PageVersion
            {
                PageId = page.Id,
                VersionNumber = 1,
                Title = page.Title,
                MetaDescription = page.MetaDescription,
                HeroHeading = page.HeroHeading,
                HeroSubheading = page.HeroSubheading,
                HeroMediaId = page.HeroMediaId,
                SecondaryMediaId = page.SecondaryMediaId,
                IsPublished = true,
                CreatedByUserId = null,
                CreatedAt = page.CreatedAt
            };
            db.PageVersions.Add(version);

            page.PublishedVersionId = version.Id;
            page.PublishedAt = page.CreatedAt;
        }

        await db.SaveChangesAsync();
    }
}
