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
            (PageSlugs.Home, "Summit Industrial Services | Scaffolding, Access, Rigging & Inspection", "Pilbara based specialist provider delivering integrated scaffolding, access, rigging, inspection and engineering support across demanding operational environments.", "Built on Trust, Driven by Excellence.", "SCAFFOLDING | ACCESS | RIGGING | INSPECTION"),
            (PageSlugs.About, "About Summit Industrial Services", "Pilbara based specialist access and rigging provider with 50+ years of combined industry experience, serving energy, resources and minerals, utilities, renewables, rail, marine and construction.", "Pilbara based specialist access & rigging.", "Summit Industrial Services  Built on Trust, Driven by Excellence."),
            (PageSlugs.Capabilities, "Our Capabilities", "Scaffolding & access solutions, scaffold design & engineering, crane & mechanical rigging, inspection & tagging, shutdown support, logistics & materials management.", "Integrated scaffolding, access, rigging & inspection.", "Traditional and modular systems  engineered solutions for complex access."),
            (PageSlugs.Contact, "Contact Summit Industrial Services", "Get in touch about scaffolding, rigging, inspection, engineering support or integrated industrial services across the Pilbara region.", "Let's talk about your project.", "1537 Pyramid Road, Karratha Industrial Estate WA 6714"),
            (PageSlugs.Industries, "Industries We Serve", "Energy, Resources & Minerals, Utilities, Renewables, Rail, and Marine & Construction  specialist access and rigging across the Greater Pilbara.", "Industries we serve.", "Access and rigging support across the Greater Pilbara."),
            (PageSlugs.Projects, "Project Support | Summit Industrial Services", "Discuss scaffolding, rigging, inspection, shutdown, maintenance and project support across the Greater Pilbara.", "Discuss your project scope.", "Pilbara based access, rigging and industrial support."),
            (PageSlugs.Insights, "Insights", "Technical notes on safety, scaffolding, rigging, inspection and industry developments from Summit's operational experience.", "Notes from the field.", "Insights from our crews, engineers and project teams.")
        ];

        var existingPages = await db.Pages.ToDictionaryAsync(p => p.Slug, p => p);

        foreach (var (slug, title, meta, hero, sub) in pages)
        {
            if (existingPages.TryGetValue(slug, out var existingPage))
            {
                if (slug == PageSlugs.Projects && existingPage.Title == "Featured Projects")
                {
                    existingPage.Title = title;
                    existingPage.MetaDescription = meta;
                    existingPage.HeroHeading = hero;
                    existingPage.HeroSubheading = sub;
                }
                else if (slug == PageSlugs.About && existingPage.MetaDescription.Contains("50+ years combined experience"))
                {
                    existingPage.MetaDescription = meta;
                }
                else if (slug == PageSlugs.Industries && existingPage.MetaDescription.Contains("Resources & Minerals Processing"))
                {
                    existingPage.MetaDescription = meta;
                    existingPage.HeroHeading = hero;
                    existingPage.HeroSubheading = sub;
                }

                continue;
            }

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
            ("company_email", "support@summit-is.com.au", "email"),
            ("company_address", "1537 Pyramid Road, Karratha Industrial Estate, WA 6714", "string"),
            ("footer_text", $"© {DateTimeOffset.UtcNow.Year} Summit Industrial Services. All rights reserved.", "string")
        ];
        var existingKeys = await db.SiteSettings.Select(s => s.Key).ToListAsync();
        foreach (var (key, value, type) in settings)
        {
            if (!existingKeys.Contains(key))
                db.SiteSettings.Add(new SiteSetting { Key = key, Value = value, ValueType = type });
        }

        var outdatedSupportEmail = await db.SiteSettings.FirstOrDefaultAsync(s =>
            s.Key == "company_email" && s.Value == "info@summit-is.com.au");
        if (outdatedSupportEmail is not null)
            outdatedSupportEmail.Value = "support@summit-is.com.au";

        if (!await db.EnquiryTypes.AnyAsync())
        {
            string[] labels = ["Project enquiry / RFP", "Shutdown or maintenance support", "Equipment hire or sales", "Engineering or design coordination", "Employment opportunities", "General enquiry"];
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
