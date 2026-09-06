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
            (PageSlugs.Home, "Summit — Industrial Construction", "Self-performed mechanical, structural, and modular construction for the country's most demanding industrial sites.", "Built by the people who show up.", "2,400+ direct-hire craft. 18M+ safe work hours. Self-perform or don't bid."),
            (PageSlugs.About, "About Summit", "Founded in 1996 on a piping contract nobody else would touch — now 2,400 craft strong.", "Self-perform or don't bid.", "The story, the values, and the people behind Summit."),
            (PageSlugs.Capabilities, "Capabilities", "Mechanical, structural, modular, underground, equipment setting, and design-assist engineering.", "Every discipline the work needs, under one roof.", "From chrome piping to constructability engineering."),
            (PageSlugs.Careers, "Careers at Summit", "Craft and professional careers building the country's largest industrial sites.", "Build the work that matters.", "Craft and professional careers, real per diem, real advancement."),
            (PageSlugs.Contact, "Contact Summit", "Get in touch about a project, a career, or a partnership.", "Let's talk about the work.", "New projects, design-assist, employment, or vendor inquiries."),
            (PageSlugs.Industries, "Industries We Serve", "Semiconductor, power, energy & terminals, renewables, and heavy manufacturing.", "Industries that can't afford to slip.", "Schedule-critical construction across five industries."),
            (PageSlugs.Projects, "Featured Projects", "480,000 LF of pipe. 900,000 BBL of storage. 7 recommissioned power units.", "The scale of work we self-perform.", "A sample of what Summit crews have delivered.")
        ];

        var existingPages = await db.Pages.ToDictionaryAsync(p => p.Slug, p => p);

        foreach (var (slug, title, meta, hero, sub) in pages)
        {
            if (existingPages.ContainsKey(slug)) continue;

            var page = new Page { Slug = slug, Title = title, MetaDescription = meta, HeroHeading = hero, HeroSubheading = sub };
            db.Pages.Add(page);
            existingPages[slug] = page;
        }

        // Backfill hero/secondary media for the two pages that originally had page-level hero art -
        // idempotent (only fills nulls) so it heals a database seeded before this field existed.
        Guid Media(string url) => mediaMap[url];

        if (existingPages.TryGetValue(PageSlugs.Home, out var homePage))
        {
            homePage.HeroMediaId ??= Media("https://summit.us/wp-content/uploads/2022/05/IMG_5350-scaled.jpg");
            homePage.SecondaryMediaId ??= Media("https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg");
        }
        if (existingPages.TryGetValue(PageSlugs.Careers, out var careersPage))
        {
            careersPage.HeroMediaId ??= Media("https://summit.us/wp-content/uploads/2022/05/22.005-Project-Hedgehog-Rio-Rancho-03.jpg");
        }

        await db.SaveChangesAsync();

        return existingPages.ToDictionary(kv => kv.Key, kv => kv.Value.Id);
    }

    private static async Task SeedSiteSettingsAndEnquiryTypesAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<SiteContentDbContext>();

        (string Key, string Value, string Type)[] settings =
        [
            ("company_name", "Summit", "string"),
            ("company_phone", "+1 (713) 555-0100", "string"),
            ("company_email", "info@summit.us", "email"),
            ("company_address", "Houston, TX", "string"),
            ("footer_text", $"© {DateTimeOffset.UtcNow.Year} Summit. All rights reserved.", "string")
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
}
