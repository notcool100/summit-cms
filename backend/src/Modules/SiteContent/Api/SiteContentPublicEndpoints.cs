using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Media.Contracts;
using SummitCms.Modules.SiteContent.Infrastructure;

namespace SummitCms.Modules.SiteContent.Api;

/// <summary>Anonymous read shape for a page's hero/SEO content, its metric stats (grouped), and global site settings.</summary>
public static class SiteContentPublicEndpoints
{
    public sealed record PublicPage(string Slug, string Title, string MetaDescription, string HeroHeading, string HeroSubheading, string? HeroMediaUrl, string? HeroMediaAlt, string? SecondaryMediaUrl, string? SecondaryMediaAlt);
    public sealed record PublicStat(string Label, decimal Value, string? Prefix, string? Suffix, string? Note);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/public/pages/{slug}", async (string slug, SiteContentDbContext db, IMediaCatalog media, CancellationToken ct) =>
        {
            var page = await db.Pages.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug, ct);
            if (page is null) return Results.NotFound();

            var mediaIds = new List<Guid>();
            if (page.HeroMediaId.HasValue) mediaIds.Add(page.HeroMediaId.Value);
            if (page.SecondaryMediaId.HasValue) mediaIds.Add(page.SecondaryMediaId.Value);
            var mediaMap = await media.GetManyAsync(mediaIds, ct);

            string? Url(Guid? id) => id.HasValue && mediaMap.TryGetValue(id.Value, out var m) ? m.Url : null;
            string? Alt(Guid? id) => id.HasValue && mediaMap.TryGetValue(id.Value, out var m) ? m.AltText : null;

            return Results.Ok(new PublicPage(
                page.Slug, page.Title, page.MetaDescription, page.HeroHeading, page.HeroSubheading,
                Url(page.HeroMediaId), Alt(page.HeroMediaId), Url(page.SecondaryMediaId), Alt(page.SecondaryMediaId)));
        }).WithTags("Public.SiteContent");

        app.MapGet("/api/public/pages/{slug}/stats", async (string slug, SiteContentDbContext db, CancellationToken ct) =>
        {
            var page = await db.Pages.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug, ct);
            if (page is null) return Results.NotFound();

            var stats = await db.MetricStats.AsNoTracking()
                .Where(s => s.PageId == page.Id)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync(ct);

            var grouped = stats
                .GroupBy(s => s.GroupKey)
                .ToDictionary(g => g.Key, g => g.Select(s => new PublicStat(s.Label, s.Value, s.Prefix, s.Suffix, s.Note)).ToList());

            return Results.Ok(grouped);
        }).WithTags("Public.SiteContent");

        app.MapGet("/api/public/site-settings", async (SiteContentDbContext db, CancellationToken ct) =>
            Results.Ok(await db.SiteSettings.AsNoTracking().ToDictionaryAsync(s => s.Key, s => s.Value, ct)))
            .WithTags("Public.SiteContent");
    }
}
