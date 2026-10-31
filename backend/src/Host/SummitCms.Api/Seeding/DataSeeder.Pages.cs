using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.SiteContent.Domain;
using SummitCms.Modules.SiteContent.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    /// <summary>
    /// Page titles, descriptions and hero copy as approved in the October 2026 client website review:
    /// unique plain-language titles, Karratha/Pilbara focus, honest "Typical Work Scopes" framing and no
    /// implied field authorship on Insights.
    /// </summary>
    internal static readonly (string Slug, string Title, string Meta, string Hero, string Sub)[] ReviewedPageCopy =
    [
        (PageSlugs.Home, "Scaffolding, Access, Rigging & Inspection in the Pilbara | Summit Industrial Services",
            "Pilbara based scaffolding, access, rigging and inspection from the Karratha Industrial Estate, supporting shutdown, maintenance, construction and marine projects across the Greater Pilbara.",
            "Pilbara-based scaffolding, access, rigging and inspection.",
            "Summit Industrial Services supports shutdown, maintenance, construction and marine projects across the Greater Pilbara through practical access solutions, rigging, inspection, engineering coordination and materials support."),
        (PageSlugs.About, "About Us | Summit Industrial Services, Karratha WA",
            "Pilbara based specialist access and rigging provider with 50+ years of combined industry experience, serving energy, resources and minerals, utilities, renewables, rail, marine and construction.",
            "Pilbara based specialist access & rigging.",
            "Summit Industrial Services. Built on Trust, Driven by Excellence."),
        (PageSlugs.Capabilities, "Scaffolding, Rigging & Inspection Capabilities | Summit Industrial Services",
            "Scaffolding and access solutions, scaffold design and engineering coordination, crane and mechanical rigging, inspection and tagging, shutdown support, and logistics and materials management in the Pilbara.",
            "Integrated scaffolding, access, rigging & inspection.",
            "Traditional and modular systems, with engineering coordination for complex access."),
        (PageSlugs.Contact, "Contact Us | Summit Industrial Services, Karratha WA",
            "Contact Summit Industrial Services at 1537 Pyramid Road, Karratha Industrial Estate WA 6714 about scaffolding, rigging, inspection or industrial support across the Greater Pilbara.",
            "Let's talk about your project.",
            "1537 Pyramid Road, Karratha Industrial Estate WA 6714"),
        (PageSlugs.Industries, "Industries We Serve in the Pilbara | Summit Industrial Services",
            "Energy, resources and minerals, utilities, renewables, rail, and marine and construction. Specialist access and rigging support across the Greater Pilbara.",
            "Industries we serve.",
            "Access and rigging support across the Greater Pilbara."),
        (PageSlugs.Projects, "Typical Work Scopes | Summit Industrial Services",
            "Capability examples showing how Summit's scaffolding, rigging, inspection and shutdown support services can be applied across the Greater Pilbara. These are illustrations, not completed client projects.",
            "Typical work scopes.",
            "Representative examples of how our services can support your project. These are capability illustrations, not individual completed client projects."),
        (PageSlugs.Insights, "Insights | Summit Industrial Services",
            "General information on safety, scaffolding, rigging, inspection and project planning from Summit Industrial Services, Karratha WA.",
            "Capability notes.",
            "General information on scaffolding, rigging, safety and project planning from Summit Industrial Services.")
    ];

    private static async Task<Dictionary<string, Guid>> SeedPagesAsync(IServiceProvider sp, Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<SiteContentDbContext>();

        (string Slug, string Title, string Meta, string Hero, string Sub)[] pages = ReviewedPageCopy;

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
            if (homePage.HeroMediaId is null || IsThirdPartyMedia(homePage.HeroMediaId)) homePage.HeroMediaId = Media(Stock("scaffold-crew-deck"));
            if (homePage.SecondaryMediaId is null || IsThirdPartyMedia(homePage.SecondaryMediaId)) homePage.SecondaryMediaId = Media(Stock("scaffold-tower-crew"));
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
